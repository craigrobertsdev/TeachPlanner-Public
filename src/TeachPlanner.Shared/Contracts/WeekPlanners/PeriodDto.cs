namespace TeachPlanner.Shared.Contracts.WeekPlanners;

public record PeriodDto(string PeriodType, string? Name, TimeOnly StartTime, TimeOnly EndTime);