using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Common.Interfaces;
using TeachPlanner.Shared.Domain.Common.Primatives;

namespace TeachPlanner.Shared.Domain.Curriculum;
public class CurriculumSubject : Entity<SubjectId>, IAggregateRoot
{
    private List<YearLevel> _yearLevels = [];
    public string Name { get; private set; } = string.Empty;
    public IReadOnlyList<YearLevel> YearLevels => _yearLevels.AsReadOnly();
    public string Description { get; private set; } = string.Empty;
    public static CurriculumSubject Create(string name, List<YearLevel> yearLevels, string description)
    {
        return new CurriculumSubject(new SubjectId(Guid.NewGuid()), name, yearLevels, description);
    }

    public static CurriculumSubject Create(SubjectId id, string name, List<YearLevel> yearLevels, string description)
    {
        return new CurriculumSubject(id, name, yearLevels, description);
    }

    public void AddYearLevel(YearLevel yearLevel)
    {
        if (YearLevels.Any(yl => yl.YearLevelValue == yearLevel.YearLevelValue))
        {
            return;
        }

        _yearLevels.Add(yearLevel);
    }
    public List<YearLevel> RemoveYearLevelsNotTaught(List<YearLevelValue> yearLevels)
    {
        var redactedYearLevels = new List<YearLevel>();
        foreach (var yearLevel in _yearLevels)
        {
            if (yearLevels.Contains(yearLevel.YearLevelValue))
            {
                redactedYearLevels.Add(yearLevel);
                continue;
            }

            var subjectYearLevels = yearLevel.GetYearLevels();
            if (yearLevels.Contains(subjectYearLevels[0]))
            {
                redactedYearLevels.Add(yearLevel);
            }
            else if (subjectYearLevels.Length > 1 && yearLevels.Contains(subjectYearLevels[1]))
            {
                redactedYearLevels.Add(yearLevel);
            }
        }

        return redactedYearLevels;
    }
    public static CurriculumSubject Create(
           SubjectId id,
           string name,
           List<YearLevel> yearLevels)
    {
        return new CurriculumSubject(
            id,
            yearLevels,
            name);
    }

    private CurriculumSubject(SubjectId id, List<YearLevel> yearLevels, string name) : base(id)
    {
        _yearLevels = yearLevels;
        Name = name;
    }

    private CurriculumSubject(SubjectId id, string name, List<YearLevel> yearLevels, string description) : base(id)
    {
        Name = name;
        _yearLevels = yearLevels;
        Description = description;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private CurriculumSubject() { }
}
public static class CurriculumSubjectExtensions
{
    public static CurriculumSubject FilterYearLevels(this CurriculumSubject subject, IEnumerable<YearLevelValue> yearLevelValues)
    {
        subject.FilterYearLevels(yearLevelValues);
        return subject;
    }

    public static CurriculumSubject FilterYearLevels(this CurriculumSubject subject, YearLevelValue yearLevelValue)
    {
        subject.FilterYearLevels(yearLevelValue);
        return subject;
    }

    public static CurriculumSubject FilterContentDescriptions(this CurriculumSubject subject, IEnumerable<Guid> contentDescriptionIds)
    {
        subject.YearLevels.FilterContentDescriptions(contentDescriptionIds);
        return subject;
    }
}
