using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Contracts.Resources;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.LessonPlans;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Shared.Contracts.LessonPlans;

public record LessonPlanDto(
    Guid LessonPlanId,
    CurriculumSubjectDto Subject,
    string PlanningNotes,
    string PlanningNotesHtml,
    List<ResourceDto> Resources,
    List<LessonCommentDto> Comments,
    int StartPeriod,
    int NumberOfPeriods);

public record LessonCommentDto(
    string Content,
    bool Completed,
    bool StruckOut,
    DateTime? CompletedDateTime);

public static class LessonPlanDtoExtensions
{
    public static List<LessonCommentDto> ToDtos(this IEnumerable<LessonComment> comments) =>
        comments.Select(c => new LessonCommentDto(c.Content, c.Completed, c.StruckOut, c.CompletedDateTime)).ToList();

    public static List<LessonPlanDto> ToDtos(this IEnumerable<LessonPlan> lessonPlans, IEnumerable<Resource> resources, IEnumerable<CurriculumSubject> subjects) =>
        lessonPlans.Select(lp => new LessonPlanDto(
            lp.Id.Value,
            lp.MatchSubject(subjects),
            lp.PlanningNotes,
            lp.PlanningNotesHtml,
            lp.MatchResources(resources).ConvertToDtos(),
            lp.Comments.ToDtos(),
            lp.StartPeriod,
            lp.NumberOfPeriods))
        .ToList();

    public static LessonPlanDto ToDto(this LessonPlan lessonPlan, IEnumerable<Resource> resources, CurriculumSubject subject)
    {
        var dto = new LessonPlanDto(
            lessonPlan.Id.Value,
            subject.ToDto(),
            lessonPlan.PlanningNotes,
            lessonPlan.PlanningNotesHtml,
            lessonPlan.MatchResources(resources).ConvertToDtos(),
            lessonPlan.Comments.ToDtos(),
            lessonPlan.StartPeriod,
            lessonPlan.NumberOfPeriods);

        return dto;
    }

    public static CurriculumSubjectDto MatchSubject(this LessonPlan lessonPlan, IEnumerable<CurriculumSubject> subjects) =>
        subjects.First(s => s.Id == lessonPlan.SubjectId).ToDto();
}