using TeachPlanner.Shared.Contracts.Resources;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.BlazorClient.Models.Resources;

public class ResourceModel : IEquatable<ResourceModel>
{
    public required ResourceId Id { get; init; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required bool IsAssessment { get; set; }
    public List<YearLevelValue> YearLevels { get; set; } = [];
    public bool Equals(ResourceModel? other) => Id.Equals(other?.Id);
    public override bool Equals(object? obj) => obj is ResourceModel other && Equals(other);
    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(ResourceModel left, ResourceModel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ResourceModel left, ResourceModel right)
    {
        return !(left == right);
    }
}

public static class ResourceModelExtensions
{
    public static List<ResourceModel> ConvertFromDtos(this IEnumerable<ResourceDto> resources) =>
        resources.Select(r => new ResourceModel
        {
            Id = r.Id,
            Name = r.Name,
            Url = r.Url,
            IsAssessment = r.IsAssessment,
            YearLevels = r.YearLevels.ToList()
        })
        .ToList();
}
