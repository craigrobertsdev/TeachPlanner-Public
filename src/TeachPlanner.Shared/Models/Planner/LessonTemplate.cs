using TeachPlanner.Shared.Interfaces.Planner;

namespace TeachPlanner.Shared.Models.Planner;

public class LessonTemplate : IPeriodTemplate
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}