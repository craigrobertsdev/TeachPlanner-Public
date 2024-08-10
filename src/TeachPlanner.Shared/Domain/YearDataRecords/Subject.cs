using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Shared.Domain.YearDataRecords;

/// <summary>
/// A teacher will have a list of subjects that are just references to the curriculum subjects
/// When a CD is added to a lesson plan or term plan, it will be added to the subject
/// When the subjects are requested, they will be queried from the curriculum subjects table
/// based on the subject name, year levels and content description ids in the _contentDescriptions list
/// the purpose of this is to reduce the size of data transferred across the wire 
/// and to create a way for a teacher to track whether they've covered the whole curriculum
/// </summary>
public record Subject
{
    private readonly List<YearDataContentDescription> _contentDescriptions = new();
    public SubjectId CurriculumSubjectId { get; private set; }
    public string Name { get; private set; }
    public IReadOnlyList<YearDataContentDescription> ContentDescriptions => _contentDescriptions.AsReadOnly();

    public void AddContentDescription(YearDataContentDescription contentDescription)
    {
        if (!_contentDescriptions.Contains(contentDescription)) _contentDescriptions.Add(contentDescription);
    }

    public static Subject Create(SubjectId curriculumSubjectId, string name, List<YearDataContentDescription> contentDescriptions)
    {
        return new Subject(curriculumSubjectId, name, contentDescriptions);
    }

    private Subject(SubjectId curriculumSubjectId, string name, List<YearDataContentDescription> contentDescriptions)
    {
        CurriculumSubjectId = curriculumSubjectId;
        Name = name;
        _contentDescriptions = contentDescriptions;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Subject()
    {
    }

}