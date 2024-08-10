using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Common.Primatives;
using TeachPlanner.Shared.Domain.LessonPlans;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Shared.Domain.Teachers;

public sealed class Resource : Entity<ResourceId>
{
    private readonly List<LessonPlan> _lessonPlans = [];
    private readonly List<YearLevelValue> _yearLevels = [];
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

        if (associatedStrands is not null) AssociatedStrands = associatedStrands;

        CreatedDateTime = DateTime.UtcNow;
        UpdatedDateTime = DateTime.UtcNow;
    }

#pragma warning disable CS8618 // non-nullable field must contain a non-null value when exiting constructor. consider declaring as nullable.
    private Resource()
    {
    }
}