namespace TeachPlanner.Api.Domain.Curriculum;

public record ConceptualOrganiser
{
    public string Name { get; set; } = string.Empty;
    public string WhatItIs { get; set; } = string.Empty;
    public string WhyItMatters { get; set; } = string.Empty;
    public List<string> ConceptualUnderstandings { get; set; } = [];
    public List<ContentDescription> ContentDescriptions { get; set; } = [];
}

public static class ConceptualOrganiserExtensions
{
    public static List<ConceptualOrganiser> FilterContentDescriptions(
        this IEnumerable<ConceptualOrganiser> conceptualOrganisers,
        IEnumerable<Guid> contentDescriptionIds)
    {
        return conceptualOrganisers.Select(s => new ConceptualOrganiser
            {
                Name = s.Name, ContentDescriptions = s.ContentDescriptions.Filter(contentDescriptionIds)
            })
            .ToList();
    }
}