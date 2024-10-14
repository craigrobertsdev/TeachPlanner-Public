using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Contracts.Resources;

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