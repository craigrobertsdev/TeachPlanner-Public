using TeachPlanner.Shared.Contracts.PlannerTemplates;

namespace TeachPlanner.Shared.Contracts.WeekPlanners;

public record CreateWeekPlannerRequest(
    int WeekNumber,
    int TermNumber,
    int Year,
    DateOnly WeekStart);
