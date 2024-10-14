namespace TeachPlanner.Shared.Interfaces.Planner;

public interface IPeriodTemplate
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}