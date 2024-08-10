using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Shared.Contracts.Curriculum;

public record CurriculumSubjectDto(
    Guid Id,
    string Name,
    IEnumerable<YearLevelDto> YearLevels);

public record YearLevelDto(
    YearLevelValue YearLevel,
    string LearningStandard,
    IEnumerable<CapabilityDto> Capabilities,
    IEnumerable<DispositionDto> Dispositions,
    IEnumerable<ConceptualOrganiserDto> ConceptualOrganisers);

public record CapabilityDto(
    string Name,
    IEnumerable<string> Descriptors);

public record DispositionDto(
    string Title,
    IEnumerable<string> DevelopedWhen);

public record ConceptualOrganiserDto(
    string Name,
    string WhatItIs,
    string WhyItMatters,
    IEnumerable<string> ConceptualUnderstandings,
    IEnumerable<ContentDescriptionDto> ContentDescriptions);

public record ContentDescriptionDto(Guid Id, string ContentDescription, string Topic, IEnumerable<string> CurriculumCodes);

public static class CurriculumSubjectDtoExtensions
{
    public static CurriculumSubjectDto ToDto(this CurriculumSubject s) =>
        new(s.Id.Value, s.Name, s.YearLevels.Select(yl => yl.ToDto()).ToList());

    public static YearLevelDto ToDto(this YearLevel yl, bool withAllInformation = true)
    {
        var capabilities = withAllInformation ? yl.Capabilities.Select(c => c.ToDto()) : new List<CapabilityDto>();
        var dispositions = withAllInformation ? yl.Dispositions.Select(d => d.ToDto()) : new List<DispositionDto>();
        var conceptualOrganisers = yl.ConceptualOrganisers.Select(co => co.ToDto(withAllInformation));

        return new(yl.YearLevelValue,
            yl.LearningStandard,
            capabilities,
            dispositions,
            conceptualOrganisers);
    }

    private static CapabilityDto ToDto(this Capability c) =>
        new(c.Name, c.Descriptors);

    private static DispositionDto ToDto(this Disposition d) =>
        new(d.Title, d.DevelopedWhen);

    public static ConceptualOrganiserDto ToDto(this ConceptualOrganiser co, bool withAllInformation = true)
    {
        if (withAllInformation)
        {
            return new(
                co.Name,
                co.WhatItIs,
                co.WhyItMatters,
                co.ConceptualUnderstandings,
                co.ContentDescriptions.Select(cd => cd.ToDto(false)));
        }
        else
        {
            return new(
                string.Empty,
                string.Empty,
                string.Empty,
                [],
                co.ContentDescriptions.Select(cd => cd.ToDto(false)));
        }

    }

    public static ContentDescriptionDto ToDto(this ContentDescription cd, bool withAllInformation = true)
    {
        var text = withAllInformation ? cd.Text : string.Empty;
        var curriculumCodes = withAllInformation ? cd.CurriculumCodes : [];

        return new(cd.Id, text, cd.ConceptualOrganiser.Name, curriculumCodes);
    }
}
