namespace TeachPlanner.Shared.Contracts.PlannerTemplates;

public record TemplatePeriodRequest(string PeriodType, string? Name, TimeOnly StartTime, TimeOnly EndTime);