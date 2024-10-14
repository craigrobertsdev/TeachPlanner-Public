using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Shared.Contracts.LessonPlans;

public record UpdateLessonPlanRequest(
    Guid LessonPlanId,
    Guid SubjectId,
    List<Guid> ContentDescriptionIds,
    string PlanningNotes,
    string PlanningNotesHtml,
    List<Guid> ResourceIds,
    DateOnly LessonDate,
    int NumberOfPeriods,
    int StartPeriod);
