using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Shared.Contracts.Resources;

public record ResourceDto(
    ResourceId Id,
    string Name,
    string Url,
    bool IsAssessment,
    IEnumerable<YearLevelValue> YearLevels);