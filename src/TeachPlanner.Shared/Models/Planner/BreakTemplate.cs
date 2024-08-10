namespace TeachPlanner.BlazorClient.Models;

public class BreakTemplate : IPeriodTemplate
{
    public string Name { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
