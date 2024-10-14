using TeachPlanner.Shared.Contracts.PlannerTemplates;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Shared.Contracts.WeekPlanners;

public record WeekPlannerDto(
    IEnumerable<DayPlanDto> DayPlans,
    WeekStructureDto WeekStructure,
    DateOnly WeekStart,
    int WeekNumber);

public record SchoolEventDto(
    Location Location,
    string Name,
    bool FullDay,
    DateTime EventStart,
    DateTime EventEnd)
{
    public static List<SchoolEventDto> CreateMany(IEnumerable<SchoolEvent> schoolEvents)
    {
        return schoolEvents.Select(se => new SchoolEventDto(
            se.Location,
            se.Name,
            se.FullDay,
            se.EventStart,
            se.EventEnd)).ToList();
    }
}