using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Common.Interfaces.Services;
using TeachPlanner.Shared.Contracts.LessonPlans.CreateLessonPlan;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.LessonPlans;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.WeekPlanners;

namespace TeachPlanner.Api.Features.LessonPlans;

public static class CreateLessonPlan
{
    public static async Task<IResult> Delegate([FromRoute] Guid teacherId, ISender sender, CreateLessonPlanRequest request,
        CancellationToken cancellationToken)
    {
        var command = new Command(
            new TeacherId(teacherId),
            new SubjectId(request.SubjectId),
            request.CondentDescriptionIds,
            request.PlanningNotes,
            request.PlanningNotesHtml,
            request.LessonDate,
            request.NumberOfPeriods,
            request.StartPeriod,
            request.ResourceIds?.Select(r => new ResourceId(r)).ToList() ?? []);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }

    public record Command(
        TeacherId TeacherId,
        SubjectId SubjectId,
        List<Guid> ContentDescriptionIds,
        string PlanningNotes,
        string PlanningNotesHtml,
        DateOnly LessonDate,
        int NumberOfPeriods,
        int StartPeriod,
        List<ResourceId> Resources) : IRequest;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.SubjectId).NotNull();
            RuleFor(x => x.NumberOfPeriods).NotEmpty();
            RuleFor(x => x.StartPeriod).NotNull();
        }
    }

    public sealed class Handler(
        ILessonPlanRepository lessonPlanRepository,
        ITeacherRepository teacherRepository,
        IWeekPlannerRepository weekPlannerRepository,
        ITermDatesService termDatesService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<Command>
    {
        private readonly ITeacherRepository _teacherRepository = teacherRepository;
        private readonly ILessonPlanRepository _lessonPlanRepository = lessonPlanRepository;
        private readonly IWeekPlannerRepository _weekPlannerRepository = weekPlannerRepository;
        private readonly ITermDatesService _termDatesService = termDatesService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetByIdWithResources(request.TeacherId, request.Resources, cancellationToken);
            if (teacher is null)
            {
                throw new TeacherNotFoundException();
            }

            var yearDataId = teacher.GetYearData(request.LessonDate.Year);
            if (yearDataId is null)
            {
                throw new YearDataNotFoundException();
            }

            var lessonPlans = await _lessonPlanRepository.GetByYearDataAndDate(yearDataId!, request.LessonDate, cancellationToken);
            var overlapExists = CheckForConflictingLessonPlans(lessonPlans, request.StartPeriod, request.NumberOfPeriods);
            var lessonPlan = lessonPlans.FirstOrDefault(lp => lp.StartPeriod == request.StartPeriod);
            var resources = await _teacherRepository.GetResourcesById(request.Resources, cancellationToken);

            using var transaction = _unitOfWork.BeginTransaction();

            if (overlapExists && lessonPlan is not null)
            {
                var lessonPlansToDelete = GetLessonPlansForDeletion(lessonPlans, request.StartPeriod, request.NumberOfPeriods);
                _lessonPlanRepository.DeleteLessonPlans(lessonPlansToDelete);

                UpdateLessonPlan(lessonPlan, request, resources);
                _lessonPlanRepository.UpdateLessonPlan(lessonPlan);
            }
            else if (!overlapExists && lessonPlan is not null)
            {
                // update the existing lesson plan
                UpdateLessonPlan(lessonPlan, request, resources);
                _lessonPlanRepository.UpdateLessonPlan(lessonPlan);
            }
            else if (overlapExists && lessonPlan is null)
            {
                var lessonPlansToDelete = GetLessonPlansForDeletion(lessonPlans, request.StartPeriod, request.NumberOfPeriods);
                _lessonPlanRepository.DeleteLessonPlans(lessonPlansToDelete);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                lessonPlan = LessonPlan.Create(
                    yearDataId,
                    request.SubjectId,
                    request.ContentDescriptionIds,
                    request.PlanningNotes,
                    request.PlanningNotesHtml,
                    request.NumberOfPeriods,
                    request.StartPeriod,
                    request.LessonDate,
                    resources);

                _lessonPlanRepository.Add(lessonPlan);

            }
            else
            {
                lessonPlan = LessonPlan.Create(
                    yearDataId,
                    request.SubjectId,
                    request.ContentDescriptionIds,
                    request.PlanningNotes,
                    request.PlanningNotesHtml,
                    request.NumberOfPeriods,
                    request.StartPeriod,
                    request.LessonDate,
                    resources);

                _lessonPlanRepository.Add(lessonPlan);
            }

            // Adding a reference to the associated dayplan to satisfy database requirement
            var termNumber = _termDatesService.GetTermNumber(request.LessonDate);
            var weekNumber = _termDatesService.GetWeekNumber(request.LessonDate.Year, termNumber, request.LessonDate);
            var weekPlanner = await _weekPlannerRepository.GetByYearAndWeekNumber(request.LessonDate.Year, weekNumber, cancellationToken);

            if (weekPlanner is null)
            {
                throw new WeekPlannerNotFoundException();
            }

            var dayPlan = weekPlanner.DayPlans.FirstOrDefault(dp => dp.Date == request.LessonDate);

            if (dayPlan is null)
            {
                dayPlan = DayPlan.Create(request.LessonDate, weekPlanner.Id, [], []);
                weekPlanner.AddDayPlan(dayPlan);
            }
            dayPlan.AddLessonPlan(lessonPlan!);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        private static IEnumerable<LessonPlan> GetLessonPlansForDeletion(IEnumerable<LessonPlan> lessonPlans, int startPeriod, int numberOfPeriods)
        {
            return lessonPlans.Where(lp => lp.StartPeriod < startPeriod + numberOfPeriods);
        }

        private static void UpdateLessonPlan(LessonPlan lessonPlan, Command request, List<Resource> resources)
        {
            lessonPlan.SetNumberOfPeriods(request.NumberOfPeriods);
            lessonPlan.SetPlanningNotes(request.PlanningNotes, request.PlanningNotesHtml);
            lessonPlan.AddCurriculumCodes(request.ContentDescriptionIds);
            lessonPlan.UpdateResources(resources);
        }
    }

    private static bool CheckForConflictingLessonPlans(List<LessonPlan> lessonPlans, int startPeriod, int numberOfPeriods)
    {
        foreach (var lp in lessonPlans)
        {
            if (StartsBeforeAndExtendsPast(lp, startPeriod, numberOfPeriods)
                || StartsAfterAndIsCoveredBy(lp, startPeriod, numberOfPeriods))
            {
                return true;
            }
        }

        return false;
    }

    private static bool StartsBeforeAndExtendsPast(LessonPlan lp, int startPeriod, int numberOfPeriods) =>
        (startPeriod < lp.StartPeriod && startPeriod + numberOfPeriods > lp.StartPeriod);

    private static bool StartsAfterAndIsCoveredBy(LessonPlan lp, int startPeriod, int numberOfPeriods) =>
        (startPeriod > lp.StartPeriod && lp.StartPeriod + lp.NumberOfPeriods > startPeriod);
}