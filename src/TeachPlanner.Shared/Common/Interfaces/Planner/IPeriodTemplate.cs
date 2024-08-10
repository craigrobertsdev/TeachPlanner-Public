namespace TeachPlanner.BlazorClient.Models;

public interface IPeriodTemplate
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
