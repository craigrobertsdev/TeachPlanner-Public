using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.Teachers.AccountSetup;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.PlannerTemplates;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Api.Features.Teachers;

public static class AccountSetup
{
    public async static Task<IResult> Delegate([FromRoute] Guid teacherId, [FromBody] AccountSetupRequest request, [FromQuery] int calendarYear, ISender sender, Validator validator, CancellationToken cancellationToken)
    {
        var weekStructure = CreateWeekStructure(request.DayPlanPattern, new TeacherId(teacherId));
        var yearLevelsTaught = request.YearLevelsTaught.Select(Enum.Parse<YearLevelValue>).ToList();
        var command = new Command(request.SubjectsTaught, yearLevelsTaught, weekStructure, new TeacherId(teacherId), calendarYear);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        await sender.Send(command);

        return Results.Ok();
    }

    public record Command(
        List<string> SubjectsTaught,
        List<YearLevelValue> YearLevelsTaught,
        WeekStructure WeekStructure,
        TeacherId TeacherId,
        int CalendarYear) : IRequest;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.SubjectsTaught).NotEmpty();
            RuleFor(x => x.WeekStructure).NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IYearDataRepository _yearDataRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(ITeacherRepository teacherRepository, ICurriculumRepository curriculumRepository, IYearDataRepository yearDataRepository, IUnitOfWork unitOfWork)
        {
            _teacherRepository = teacherRepository;
            _curriculumRepository = curriculumRepository;
            _yearDataRepository = yearDataRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetById(request.TeacherId, cancellationToken);

            if (teacher == null)
            {
                throw new TeacherNotFoundException();
            }

            var subjectsTaught = await _curriculumRepository.GetSubjectsByName(request.SubjectsTaught, cancellationToken);
            ValidateSubjects(subjectsTaught, request.SubjectsTaught);
            teacher.SetSubjectsTaught(subjectsTaught);
            teacher.CompleteAccountSetup();

            await _yearDataRepository.SetInitialAccountDetails(teacher, request.YearLevelsTaught, request.WeekStructure, request.CalendarYear, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

    }
    private static void ValidateSubjects(List<CurriculumSubject> subjects, List<string> subjectNames)
    {
        if (subjects.Count != subjectNames.Count)
        {
            throw new SubjectNotFoundException(subjectNames.Except(subjects.Select(s => s.Name)).First());
        }
    }

    private static WeekStructure CreateWeekStructure(DayPlanPatternDto dayPlanPattern, TeacherId teacherId)
    {
        var templatePeriods = CreateTemplatePeriods(dayPlanPattern);
        templatePeriods.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));

        ValidateTemplatePeriodTimes(templatePeriods);

        return WeekStructure.Create(templatePeriods, teacherId);
    }

    private static List<TemplatePeriod> CreateTemplatePeriods(DayPlanPatternDto dayPlanPattern)
    {
        List<TemplatePeriod> periods = new();

        for (int i = 0; i < dayPlanPattern.LessonTemplates.Count; i++)
        {
            periods.Add(new TemplatePeriod(
                PeriodType.Lesson,
                $"Lesson {i + 1}",
                dayPlanPattern.LessonTemplates[i].StartTime,
                dayPlanPattern.LessonTemplates[i].EndTime));
        }

        for (int i = 0; i < dayPlanPattern.BreakTemplates.Count; i++)
        {
            periods.Add(new TemplatePeriod(
                PeriodType.Break,
                dayPlanPattern.BreakTemplates[i].Name,
                dayPlanPattern.BreakTemplates[i].StartTime,
                dayPlanPattern.BreakTemplates[i].EndTime));
        }

        return periods;
    }

    private static void ValidateTemplatePeriodTimes(List<TemplatePeriod> templatePeriods)
    {
        for (int i = 0; i < templatePeriods.Count - 1; i++)
        {
            if (templatePeriods[i].EndTime != templatePeriods[i + 1].StartTime)
            {
                throw new CreateTimeFromDtoException("Lesson and break start and end times must be continuous");
            }

            if (templatePeriods[i].EndTime > templatePeriods[i + 1].StartTime)
            {
                throw new CreateTimeFromDtoException("Lesson and break times must not overlap");
            }
        }
    }
}
