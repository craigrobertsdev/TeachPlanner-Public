namespace TeachPlanner.BlazorClient.Models;

public class LessonTemplate : IPeriodTemplate
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
