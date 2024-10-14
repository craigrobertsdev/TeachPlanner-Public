using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Interfaces.Services;
using TeachPlanner.Shared.Contracts.LessonPlans;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.LessonPlans;

public static class UpdateLessonPlan 
{
    public static async Task<IResult> Endpoint([FromRoute] Guid teacherId, UpdateLessonPlanRequest request, ISender sender, CancellationToken cancellationToken)
    {
        var command = new Command(
            new TeacherId(teacherId),
            new LessonPlanId(request.LessonPlanId),
            new SubjectId(request.SubjectId),
            request.ContentDescriptionIds,
            request.PlanningNotes,
            request.PlanningNotesHtml,
            request.LessonDate,
            request.NumberOfPeriods,
            request.StartPeriod,
            request.ResourceIds.Select(r => new ResourceId(r)).ToList());

        await sender.Send(command, cancellationToken);
        
        return Results.Ok();
    }

    public record Command(
        TeacherId TeacherId,
        LessonPlanId LessonPlanId,
        SubjectId SubjectId,
        List<Guid> ContentDescriptionIds,
        string PlanningNotes,
        string PlanningNotesHtml,
        DateOnly LessonDate,
        int NumberOfPeriods,
        int StartPeriod,
        List<ResourceId> Resources) : IRequest;

    public sealed class Handler(
        ILessonPlanRepository lessonPlanRepository,
        ITeacherRepository teacherRepository,
        IWeekPlannerRepository weekPlannerRepository,
        ITermDatesService termDatesService,
        IUnitOfWork unitOfWork) : IRequestHandler<Command>
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
            var lessonPlan = lessonPlans.FirstOrDefault(lp => lp.Id == request.LessonPlanId);
            if (lessonPlan is null)
            {
                throw new LessonPlansNotFoundException();
            }
            var resources = await teacherRepository.GetResourcesById(request.Resources, cancellationToken);
            
            // await using var transaction = unitOfWork.BeginTransaction();

            if (overlapWillExist)
            {
                var lessonPlansToDelete =
                    GetLessonPlansForDeletion(lessonPlans, request.StartPeriod, request.NumberOfPeriods);
                lessonPlanRepository.DeleteLessonPlans(lessonPlansToDelete);
            }

            UpdateLessonPlan(lessonPlan, request, resources);
            lessonPlanRepository.UpdateLessonPlan(lessonPlan);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            // await transaction.CommitAsync(cancellationToken);
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

        private static bool StartsAfterAndIsCoveredBy(LessonPlan lp, int startPeriod, int numberOfPeriods)
        {
            return startPeriod > lp.StartPeriod && lp.StartPeriod + lp.NumberOfLessons > startPeriod;
        }

        private static bool StartsBeforeAndExtendsPast(LessonPlan lp, int startPeriod, int numberOfPeriods)
        {
            return startPeriod < lp.StartPeriod && startPeriod + numberOfPeriods > lp.StartPeriod;
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
            lessonPlan.SetStartPeriod(request.StartPeriod);
            lessonPlan.SetPlanningNotes(request.PlanningNotes, request.PlanningNotesHtml);
            lessonPlan.SetCurriculumCodes(request.ContentDescriptionIds);
            lessonPlan.UpdateResources(resources);
            lessonPlan.UpdateSubject(request.SubjectId);
        }
    }
}