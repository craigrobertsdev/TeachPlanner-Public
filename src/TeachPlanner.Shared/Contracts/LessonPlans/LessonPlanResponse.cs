using TeachPlanner.Shared.Contracts.Curriculum;

namespace TeachPlanner.Shared.Contracts.LessonPlans;

public record LessonPlanResponse(
    LessonPlanDto? LessonPlan,
    List<CurriculumSubjectDto> Curriculum);