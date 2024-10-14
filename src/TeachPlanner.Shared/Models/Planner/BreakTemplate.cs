using TeachPlanner.Shared.Interfaces.Planner;

namespace TeachPlanner.Shared.Models.Planner;

public class BreakTemplate : IPeriodTemplate
{
    public string Name { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}