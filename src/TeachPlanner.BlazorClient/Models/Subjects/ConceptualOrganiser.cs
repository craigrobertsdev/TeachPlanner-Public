using TeachPlanner.Shared.Contracts.Curriculum;

namespace TeachPlanner.BlazorClient.Models.Subjects;

public record ConceptualOrganiser
{
    public string Name { get; set; } = string.Empty;
    public List<ContentDescription> ContentDescriptions { get; set; } = [];
}

public static class ConceptualOrganiserExtensions
{
    public static List<ConceptualOrganiser> ConvertFromDtos(
        this IEnumerable<ConceptualOrganiserDto> conceptualOrganisers)
    {
        return conceptualOrganisers.Select(s => new ConceptualOrganiser
            {
                Name = s.Name, ContentDescriptions = s.ContentDescriptions.ConvertFromDtos()
            })
            .ToList();
    }
}