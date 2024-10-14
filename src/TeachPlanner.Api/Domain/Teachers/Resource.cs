using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Shared.Contracts.Resources;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.Teachers;

public sealed class Resource : Entity<ResourceId>
{
    private readonly List<LessonPlan> _lessonPlans = [];
    private readonly List<YearLevelValue> _yearLevels = [];

    private Resource(
        TeacherId teacherId,
        ResourceId id,
        string name,
        string url,
        bool isAssessment,
        SubjectId subjectId,
        List<YearLevelValue> yearLevels,
        List<string>? associatedStrands = null) : base(id)
    {
        TeacherId = teacherId;
        Name = name;
        Url = url;
        IsAssessment = isAssessment;
        SubjectId = subjectId;
        _yearLevels = yearLevels;

        if (associatedStrands is not null)
        {
            AssociatedStrands = associatedStrands;
        }

        CreatedDateTime = DateTime.UtcNow;
        UpdatedDateTime = DateTime.UtcNow;
    }

    public TeacherId TeacherId { get; private set; }
    public string Name { get; private set; }
    public string Url { get; private set; }
    public bool IsAssessment { get; private set; }
    public IReadOnlyList<LessonPlan> LessonPlans => _lessonPlans.AsReadOnly();
    public IReadOnlyList<YearLevelValue> YearLevels => _yearLevels.AsReadOnly();
    public SubjectId SubjectId { get; private set; }
    public List<string> AssociatedStrands { get; private set; } = [];
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    public static Resource Create(
        TeacherId teacherId,
        string name,
        string url,
        bool isAssessment,
        SubjectId subjectId,
        List<YearLevelValue> yearLevels,
        List<string>? strandNames)
    {
        var resource = new Resource(
            teacherId,
            new ResourceId(Guid.NewGuid()),
            name,
            url,
            isAssessment,
            subjectId,
            yearLevels,
            strandNames);

        return resource;
    }
#pragma warning disable CS8618 // non-nullable field must contain a non-null value when exiting constructor. consider declaring as nullable.
    private Resource()
    {
    }
}

public static class ResourceDtoExtensions
{
    public static ResourceDto ConvertToDto(this Resource resource)
    {
        return new ResourceDto(resource.Id, resource.Name, resource.Url, resource.IsAssessment, resource.YearLevels);
    }

    public static List<ResourceDto> ConvertToDtos(this IEnumerable<Resource> resources)
    {
        return resources.Select(r => new ResourceDto(r.Id, r.Name, r.Url, r.IsAssessment, r.YearLevels)).ToList();
    }
}