using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Interfaces.Services;
using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Contracts.LessonPlans;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.LessonPlans;

/// <summary>
///     Returns the data associated with a lessonPlan if it exists, otherwise returns the subjects for the teacher to
///     create a new one
/// </summary>
public static class GetLessonPlan
{
    public static async Task<IResult> Endpoint([FromRoute] Guid teacherId, DateOnly lessonDate, int period,
        bool isNewLesson, ISender sender, CancellationToken cancellationToken)
    {
        var query = new Query(new TeacherId(teacherId), lessonDate, period, isNewLesson);

        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }

    public record Query(TeacherId TeacherId, DateOnly LessonDate, int Period, bool IsNewLesson)
        : IRequest<LessonPlanResponse>;

    public sealed class Handler(
        ITeacherRepository teacherRepository,
        IYearDataRepository yearDataRepository,
        ICurriculumService curriculumService,
        ILessonPlanRepository lessonPlanRepository) : IRequestHandler<Query, LessonPlanResponse>
    {
        public async Task<LessonPlanResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            var teacher = await teacherRepository.GetById(request.TeacherId, cancellationToken) ??
                          throw new TeacherNotFoundException();
            var lessonPlan = await lessonPlanRepository.GetByYearDataAndDateAndPeriod(teacher.YearDataHistory
                    .FirstOrDefault(ydh => ydh.CalendarYear == request.LessonDate.Year)!.YearDataId, request.LessonDate,
                request.Period, cancellationToken);

            if (!request.IsNewLesson && lessonPlan is null)
            {
                throw new LessonPlansNotFoundException();
            }

            var yearLevels =
                await yearDataRepository.GetYearLevelsTaught(request.TeacherId, request.LessonDate.Year,
                    cancellationToken);
            var subjects = curriculumService.GetSubjectsByYearLevels(teacher.SubjectsTaught, yearLevels);

            // No year levels required for initial lesson response.
            // They will be requested by the client when the user selects a subject and wants to add content descriptions.
            var subjectDtos = subjects.Select(s => new CurriculumSubjectDto(s.Id.Value, s.Name, [])).ToList();

            if (lessonPlan is null)
            {
                return new LessonPlanResponse(null, subjectDtos);
            }

            var resources = await lessonPlanRepository.GetResources(lessonPlan, cancellationToken);
            var subject = subjects.First(s => s.Id == lessonPlan.SubjectId);

            return new LessonPlanResponse(
                lessonPlan!.ToDto(
                    resources,
                    subject.FilterContentDescriptions(lessonPlan.ContentDescriptionIds)),
                subjectDtos);
        }
    }
}