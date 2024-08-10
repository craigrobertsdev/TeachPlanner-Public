using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Common.Interfaces.Services;
using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Contracts.LessonPlans;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Api.Features.LessonPlans;
/// <summary>
/// Returns the data associated with a lessonplan if it exists, otherwise returns the subjects for the teacher to create a new one
/// </summary>
public static class GetLessonPlan
{
    public record Query(TeacherId TeacherId, DateOnly LessonDate, int Period, bool IsNewLesson) : IRequest<LessonPlanResponse>;

    public sealed class Handler(
        ITeacherRepository teacherRepository,
        IYearDataRepository yearDataRepository,
        ICurriculumService curriculumService,
        ILessonPlanRepository lessonPlanRepository) : IRequestHandler<Query, LessonPlanResponse>
    {
        private readonly ITeacherRepository _teacherRepository = teacherRepository;
        private readonly IYearDataRepository _yearDataRepository = yearDataRepository;
        private readonly ILessonPlanRepository _lessonPlanRepository = lessonPlanRepository;
        private readonly ICurriculumService _curriculumService = curriculumService;

        public async Task<LessonPlanResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetById(request.TeacherId, cancellationToken) ?? throw new TeacherNotFoundException();
            var lessonPlan = await _lessonPlanRepository.GetByYearDataAndDateAndPeriod(teacher.YearDataHistory
                .FirstOrDefault(ydh => ydh.CalendarYear == request.LessonDate.Year)!.YearDataId, request.LessonDate, request.Period, cancellationToken);

            if (!request.IsNewLesson && lessonPlan is null)
            {
                throw new LessonPlansNotFoundException();
            }

            var yearLevels = await _yearDataRepository.GetYearLevelsTaught(request.TeacherId, request.LessonDate.Year, cancellationToken);
            var subjects = _curriculumService.GetSubjectsByYearLevels(teacher.SubjectsTaught, yearLevels);

            // No year levels required for initial lesson response.
            // They will be requested by the client when the user selects a subject and wants to add content descriptions.
            var subjectDtos = subjects.Select(s => new CurriculumSubjectDto(s.Id.Value, s.Name, [])).ToList();

            if (lessonPlan is null)
            {
                return new LessonPlanResponse(null, subjectDtos);
            }

            var resources = await _lessonPlanRepository.GetResources(lessonPlan, cancellationToken);
            var subject = subjects.First(s => s.Id == lessonPlan.SubjectId);

            return new LessonPlanResponse(
                lessonPlan!.ToDto(
                    resources,
                    subject.FilterContentDescriptions(lessonPlan.ContentDescriptionIds)),
                subjectDtos);
        }
    }

    public static async Task<IResult> Delegate([FromRoute] Guid teacherId, DateOnly lessonDate, int period, bool isNewLesson, ISender sender, CancellationToken cancellationToken)
    {
        var query = new Query(new TeacherId(teacherId), lessonDate, period, isNewLesson);

        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }
}
