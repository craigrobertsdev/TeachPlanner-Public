using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.BlazorClient.State;

public class TeacherStateContainer
{
    public TeacherId Id { get; set; } = null!;
    public string FirstName { get; set; } = string.Empty;
    public bool AccountSetupComplete { get; set; }
    public int LastSelectedYear { get; set; }
    public DateOnly LastSelectedWeekStart { get; set; }
}