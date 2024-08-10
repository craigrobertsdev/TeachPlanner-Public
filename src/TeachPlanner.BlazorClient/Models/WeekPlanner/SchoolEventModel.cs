using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Domain.Common;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public record SchoolEventModel
{
    public Location Location { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public bool FullDay { get; set; }
    public DateTime EventStart { get; set; }
    public DateTime EventEnd { get; set; }
}

public static class SchoolEventModelExtensions
{
    public static IEnumerable<SchoolEventModel> ConvertFromDtos(this IEnumerable<SchoolEventDto> schoolEvents) =>
        schoolEvents.Select(se => new SchoolEventModel
        {
            Location = se.Location,
            Name = se.Name,
            FullDay = se.FullDay,
            EventStart = se.EventStart,
            EventEnd = se.EventEnd
        });
}
