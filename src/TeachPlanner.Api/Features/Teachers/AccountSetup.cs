using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.WeekPlanners;
using TeachPlanner.Api.Domain.YearDataRecords;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Interfaces.Services;
using TeachPlanner.Shared.Contracts.Teachers.AccountSetup;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.Features.Teachers;

public static class AccountSetup
{
    public static async Task<IResult> Endpoint([FromRoute] Guid teacherId, [FromBody] AccountSetupRequest request,
        [FromQuery] int calendarYear, ISender sender, Validator validator,
        ICurriculumService curriculumService,
        ITeacherRepository teacherRepository, IUnitOfWork unitOfWork, IYearDataRepository yearDataRepository, ITermDatesService termDatesService,
        CancellationToken cancellationToken)
    {
        var stronglyTypedTeacherId = new TeacherId(teacherId);
        var weekStructure = request.WeekStructure.FromDto(stronglyTypedTeacherId);
        var yearLevelsTaught = request.YearLevelsTaught.Select(Enum.Parse<YearLevelValue>).ToList();
        var command = new Command(request.SubjectsTaught, yearLevelsTaught, weekStructure, stronglyTypedTeacherId,
            calendarYear);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        // await sender.Send(command, cancellationToken);
        var handler = new Handler(teacherRepository, curriculumService, yearDataRepository, unitOfWork, termDatesService);
        await handler.Handle(command, cancellationToken);
        return Results.Ok();
    }

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly ICurriculumService _curriculumService;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITermDatesService _termDatesService;
        private readonly IYearDataRepository _yearDataRepository;

        public Handler(ITeacherRepository teacherRepository, ICurriculumService curriculumService,
            IYearDataRepository yearDataRepository, IUnitOfWork unitOfWork, ITermDatesService termDatesService)
        {
            _teacherRepository = teacherRepository;
            _curriculumService = curriculumService;
            _yearDataRepository = yearDataRepository;
            _unitOfWork = unitOfWork;
            _termDatesService = termDatesService;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetById(request.TeacherId, cancellationToken);

            if (teacher == null)
            {
                throw new TeacherNotFoundException();
            }

            var subjectsTaught = _curriculumService.GetSubjectsByName(request.SubjectsTaught);
            ValidateSubjects(subjectsTaught, request.SubjectsTaught);
            teacher.SetSubjectsTaught(subjectsTaught.Select(s => s.Id).ToList());
            _teacherRepository.Update(teacher);
            // await _unitOfWork.SaveChangesAsync(cancellationToken);

            var yearData =
                await _yearDataRepository.GetByTeacherIdAndYear(teacher.Id, request.CalendarYear, cancellationToken);
            if (yearData == null)
            {
                throw new YearDataNotFoundException();
            }

            UpdateWeekStructure(yearData, request.WeekStructure, teacher.Id);

            await _yearDataRepository.SetInitialAccountDetails(teacher, request.YearLevelsTaught, request.WeekStructure,
                request.CalendarYear, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var firstDayOfYear = _termDatesService.GetWeekStart(request.CalendarYear, 1, 1);
            // Makes sure everything worked. Once teacher.AccountSetupComplete == true, the user cannot go back to the account setup page
            teacher.CompleteAccountSetup(request.CalendarYear, firstDayOfYear);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private static void ValidateSubjects(List<CurriculumSubject> subjects, List<string> subjectNames)
    {
        var curriculumSubjectNames = subjects.Select(subject => subject.Name).ToList();
        if (subjectNames.Any(subjectName => !curriculumSubjectNames.Contains(subjectName)))
        {
            throw new SubjectNotFoundException(subjectNames.Except(subjects.Select(s => s.Name)).First());
        }
    }

    private static void UpdateWeekStructure(YearData yearData, WeekStructure weekStructure, TeacherId teacherId)
    {
        weekStructure.SortPeriods();
        ValidateTemplatePeriodTimes((IList<TemplatePeriod>)weekStructure.Periods);

        yearData.WeekStructure.SetDayTemplates(weekStructure.DayTemplates);
        yearData.WeekStructure.SetPeriods(weekStructure.Periods);
    }

    private static void ValidateTemplatePeriodTimes(IList<TemplatePeriod> templatePeriods)
    {
        for (var i = 0; i < templatePeriods.Count - 1; i++)
        {
            if (templatePeriods[i].EndTime <= templatePeriods[i].StartTime)
            {
                throw new CreateTimeFromDtoException("Start time must be before end time");
            }
            
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
}