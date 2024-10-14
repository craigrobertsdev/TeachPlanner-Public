using TeachPlanner.Shared.Enums;

namespace TeachPlanner.Shared.ValueObjects;

/// <summary>
///     Represents an entry in a day's schedule, which can be a lesson, a break or NIT.
/// </summary>
/// <param name="PeriodType"></param>
/// <param name="Name">"Recess" or "lunch"</param>
/// <param name="StartTime"></param>
/// <param name="EndTime"></param>
public class TemplatePeriod(PeriodType periodType, string? name, TimeOnly startTime, TimeOnly endTime)
{
    public PeriodType PeriodType { get; set; } = periodType;
    public string? Name { get; set; } = name;
    public TimeOnly StartTime { get; set; } = startTime;
    public TimeOnly EndTime { get; set; } = endTime;
}