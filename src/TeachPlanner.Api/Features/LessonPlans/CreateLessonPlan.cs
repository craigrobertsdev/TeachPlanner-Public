using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.WeekPlanners;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Interfaces.Services;
using TeachPlanner.Shared.Contracts.LessonPlans.CreateLessonPlan;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.LessonPlans;

public static class CreateLessonPlan
{
    public static async Task<IResult> Endpoint([FromRoute] Guid teacherId, ISender sender,
        CreateLessonPlanRequest request,
        CancellationToken cancellationToken)
    {
        var command = new Command(
            new TeacherId(teacherId),
            new SubjectId(request.SubjectId),
            request.ContentDescriptionIds,
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
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var teacher =
                await teacherRepository.GetByIdWithResources(request.TeacherId, request.Resources, cancellationToken);
            if (teacher is null)
            {
                throw new TeacherNotFoundException();
            }

            var yearDataId = teacher.GetYearData(request.LessonDate.Year);
            if (yearDataId is null)
            {
                throw new YearDataNotFoundException();
            }

            var lessonPlans =
                await lessonPlanRepository.GetByYearDataAndDate(yearDataId, request.LessonDate, cancellationToken);
            var overlapWillExist =
                CheckForConflictingLessonPlans(lessonPlans, request.StartPeriod, request.NumberOfPeriods);
            var lessonPlan = lessonPlans.FirstOrDefault(lp => lp.StartPeriod == request.StartPeriod);
            var resources = await teacherRepository.GetResourcesById(request.Resources, cancellationToken);

            await using var transaction = unitOfWork.BeginTransaction();

            if (overlapWillExist)
            {
                var lessonPlansToDelete =
                    GetLessonPlansForDeletion(lessonPlans, request.StartPeriod, request.NumberOfPeriods);
                lessonPlanRepository.DeleteLessonPlans(lessonPlansToDelete);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

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
            lessonPlanRepository.Add(lessonPlan);
            await UpdateWeekPlannerRelationships(request.LessonDate, lessonPlan, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        private static bool CheckForConflictingLessonPlans(List<LessonPlan> lessonPlans, int startPeriod,
            int numberOfPeriods)
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

        private static bool StartsBeforeAndExtendsPast(LessonPlan lp, int startPeriod, int numberOfPeriods)
        {
            return startPeriod < lp.StartPeriod && startPeriod + numberOfPeriods > lp.StartPeriod;
        }

        private static bool StartsAfterAndIsCoveredBy(LessonPlan lp, int startPeriod, int numberOfPeriods)
        {
            return startPeriod > lp.StartPeriod && lp.StartPeriod + lp.NumberOfLessons > startPeriod;
        }

        private async Task UpdateWeekPlannerRelationships(DateOnly lessonDate, LessonPlan lessonPlan,
            CancellationToken cancellationToken)
        {
            // Adding a reference to the associated dayPlan to satisfy database requirement
            var termNumber = termDatesService.GetTermNumber(lessonDate);
            var weekNumber =
                termDatesService.GetWeekNumber(lessonDate.Year, termNumber, lessonDate);
            var weekPlanner =
                await weekPlannerRepository.GetByYearAndWeekNumber(lessonDate.Year, weekNumber,
                    cancellationToken);

            if (weekPlanner is null)
            {
                throw new WeekPlannerNotFoundException();
            }

            var dayPlan = weekPlanner.DayPlans.FirstOrDefault(dp => dp.Date == lessonDate);

            if (dayPlan is null)
            {
                dayPlan = DayPlan.Create(lessonDate, weekPlanner.Id, [], []);
                weekPlanner.UpdateDayPlan(dayPlan);
            }

            dayPlan.AddLessonPlan(lessonPlan!);
        }

        private static IEnumerable<LessonPlan> GetLessonPlansForDeletion(IEnumerable<LessonPlan> lessonPlans,
            int startPeriod, int numberOfPeriods)
        {
            return lessonPlans.Where(lp =>
                lp.StartPeriod >= startPeriod && lp.StartPeriod < startPeriod + numberOfPeriods);
        }

        private static void UpdateLessonPlan(LessonPlan lessonPlan, Command request, List<Resource> resources)
        {
            lessonPlan.SetNumberOfLessons(request.NumberOfPeriods);
            lessonPlan.SetPlanningNotes(request.PlanningNotes, request.PlanningNotesHtml);
            lessonPlan.SetCurriculumCodes(request.ContentDescriptionIds);
            lessonPlan.UpdateResources(resources);
        }
    }
}