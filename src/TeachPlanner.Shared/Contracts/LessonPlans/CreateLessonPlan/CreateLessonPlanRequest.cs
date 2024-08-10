namespace TeachPlanner.Shared.Contracts.LessonPlans.CreateLessonPlan;

public record CreateLessonPlanRequest(
    Guid SubjectId,
    List<Guid> CondentDescriptionIds,
    string PlanningNotes,
    string PlanningNotesHtml,
    List<Guid>? ResourceIds,
    DateOnly LessonDate,
    int NumberOfPeriods,
    int StartPeriod);