using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.BlazorClient.State;

public class TeacherStateContainer
{
    public TeacherId Id { get; set; } = null!;
    public string FirstName { get; set; } = string.Empty;
    public bool AccountSetupComplete { get; set; }
}
