using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public record SchoolEvent
{
    public Location Location { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public bool FullDay { get; set; }
    public DateTime EventStart { get; set; }
    public DateTime EventEnd { get; set; }
}

public static class SchoolEventExtensions
{
    public static IEnumerable<SchoolEvent> ConvertFromDtos(this IEnumerable<SchoolEventDto> schoolEvents)
    {
        return schoolEvents.Select(se => new SchoolEvent
        {
            Location = se.Location,
            Name = se.Name,
            FullDay = se.FullDay,
            EventStart = se.EventStart,
            EventEnd = se.EventEnd
        });
    }
}