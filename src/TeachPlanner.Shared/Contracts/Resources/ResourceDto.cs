using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Shared.Contracts.Resources;

public record ResourceDto(
    ResourceId Id,
    string Name,
    string Url,
    bool IsAssessment,
    IEnumerable<YearLevelValue> YearLevels);

public static class ResourceDtoExtensions
{
    public static ResourceDto ConvertToDto(this Resource resource) =>
        new ResourceDto(resource.Id, resource.Name, resource.Url, resource.IsAssessment, resource.YearLevels);

    public static List<ResourceDto> ConvertToDtos(this IEnumerable<Resource> resources) =>
        resources.Select(r => new ResourceDto(r.Id, r.Name, r.Url, r.IsAssessment, r.YearLevels)).ToList();
}