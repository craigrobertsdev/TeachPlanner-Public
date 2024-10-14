using TeachPlanner.Shared.Contracts.PlannerTemplates;

namespace TeachPlanner.Shared.Contracts.Teachers.AccountSetup;

public record AccountSetupRequest(
    List<string> SubjectsTaught,
    List<string> YearLevelsTaught,
    WeekStructureDto WeekStructure);