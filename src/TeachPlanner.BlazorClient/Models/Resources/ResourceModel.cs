using TeachPlanner.Shared.Contracts.Resources;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.BlazorClient.Models.Resources;

public class Resource : IEquatable<Resource>
{
    public required ResourceId Id { get; init; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required bool IsAssessment { get; set; }
    public List<YearLevelValue> YearLevels { get; set; } = [];

    public bool Equals(Resource? other)
    {
        return Id.Equals(other?.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Resource other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Resource left, Resource right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Resource left, Resource right)
    {
        return !(left == right);
    }
}

public static class ResourceExtensions
{
    public static List<Resource> ConvertFromDtos(this IEnumerable<ResourceDto> resources)
    {
        return resources.Select(r => new Resource
            {
                Id = r.Id,
                Name = r.Name,
                Url = r.Url,
                IsAssessment = r.IsAssessment,
                YearLevels = r.YearLevels.ToList()
            })
            .ToList();
    }
}