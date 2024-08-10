using TeachPlanner.Shared.Contracts.WeekPlanners;

namespace TeachPlanner.Shared.Domain.PlannerTemplates;

/// <summary>
/// Represents an entry in a day's schedule, which can be a lesson, a break or NIT.
/// </summary>
/// <param name="PeriodType"></param>
/// <param name="Name"></param>
/// <param name="StartTime"></param>
/// <param name="EndTime"></param>
public class TemplatePeriod(PeriodType periodType, string name, TimeOnly startTime, TimeOnly endTime)
{
    public PeriodType PeriodType { get; set; } = periodType;
    public string Name { get; set; } = name;
    public TimeOnly StartTime { get; set; } = startTime;
    public TimeOnly EndTime { get; set; } = endTime;
}

public static class TemplatePeriodExtensions
{
    public static List<PeriodDto> ToDto(this IEnumerable<TemplatePeriod> templatePeriods) =>
        templatePeriods.Select(
            period => new PeriodDto(
            period.PeriodType.ToString(),
            period.Name ?? null,
            period.StartTime,
            period.EndTime)).ToList();
}