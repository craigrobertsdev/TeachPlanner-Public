namespace TeachPlanner.Shared.Contracts.WeekPlanners;

public record CreateWeekPlannerRequest(
    int WeekNumber,
    int TermNumber,
    int Year);