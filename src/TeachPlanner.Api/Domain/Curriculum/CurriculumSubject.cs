using TeachPlanner.Api.Domain.Common.Interfaces;
using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.Curriculum;

public class CurriculumSubject : Entity<SubjectId>, IAggregateRoot
{
    private readonly List<YearLevel> _yearLevels = [];

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

    public string Name { get; } = string.Empty;
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private CurriculumSubject()
    {
    }
}

public static class CurriculumSubjectExtensions
{
    public static CurriculumSubject FilterYearLevels(this CurriculumSubject subject,
        IEnumerable<YearLevelValue> yearLevelValues)
    {
        subject.FilterYearLevels(yearLevelValues);
        return subject;
    }

    public static CurriculumSubject FilterYearLevels(this CurriculumSubject subject, YearLevelValue yearLevelValue)
    {
        subject.FilterYearLevels(yearLevelValue);
        return subject;
    }

    public static CurriculumSubject FilterContentDescriptions(this CurriculumSubject subject,
        IEnumerable<Guid> contentDescriptionIds)
    {
        subject.YearLevels.FilterContentDescriptions(contentDescriptionIds);
        return subject;
    }
}

public static class CurriculumSubjectDtoExtensions
{
    public static CurriculumSubjectDto ToDto(this CurriculumSubject s)
    {
        return new CurriculumSubjectDto(s.Id.Value, s.Name, s.YearLevels.Select(yl => yl.ToDto()).ToList());
    }

    public static YearLevelDto ToDto(this YearLevel yl, bool withAllInformation = true)
    {
        var capabilities = withAllInformation ? yl.Capabilities.Select(c => c.ToDto()) : new List<CapabilityDto>();
        var dispositions = withAllInformation ? yl.Dispositions.Select(d => d.ToDto()) : new List<DispositionDto>();
        var conceptualOrganisers = yl.ConceptualOrganisers.Select(co => co.ToDto(withAllInformation));

        return new YearLevelDto(yl.YearLevelValue,
            yl.LearningStandard,
            capabilities,
            dispositions,
            conceptualOrganisers);
    }

    private static CapabilityDto ToDto(this Capability c)
    {
        return new CapabilityDto(c.Name, c.Descriptors);
    }

    private static DispositionDto ToDto(this Disposition d)
    {
        return new DispositionDto(d.Title, d.DevelopedWhen);
    }

    public static ConceptualOrganiserDto ToDto(this ConceptualOrganiser co, bool withAllInformation = true)
    {
        if (withAllInformation)
        {
            return new ConceptualOrganiserDto(
                co.Name,
                co.WhatItIs,
                co.WhyItMatters,
                co.ConceptualUnderstandings,
                co.ContentDescriptions.Select(cd => cd.ToDto(false)));
        }

        return new ConceptualOrganiserDto(
            string.Empty,
            string.Empty,
            string.Empty,
            [],
            co.ContentDescriptions.Select(cd => cd.ToDto(false)));
    }

    public static ContentDescriptionDto ToDto(this ContentDescription cd, bool withAllInformation = true)
    {
        var text = withAllInformation ? cd.Text : string.Empty;
        var curriculumCodes = withAllInformation ? cd.CurriculumCodes : [];

        return new ContentDescriptionDto(cd.Id, text, cd.ConceptualOrganiser.Name, curriculumCodes);
    }
}