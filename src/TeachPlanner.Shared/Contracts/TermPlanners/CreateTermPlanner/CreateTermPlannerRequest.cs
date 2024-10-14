using TeachPlanner.Shared.Enums;

namespace TeachPlanner.Shared.Contracts.TermPlanners.CreateTermPlanner;

public record CreateTermPlannerRequest(List<TermPlannerDto> TermPlans, List<YearLevelValue> YearLevels);