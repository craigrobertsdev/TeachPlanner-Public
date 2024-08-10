using TeachPlanner.Shared.Contracts.Curriculum;

namespace TeachPlanner.BlazorClient.Models.Subjects;

public record ConceptualOrganiserModel
{
    public string Name { get; set; } = string.Empty;
    public List<ContentDescriptionModel> ContentDescriptions { get; set; } = [];
}

public static class StrandModelExtensions
{
    public static List<ConceptualOrganiserModel> ConvertFromDtos(this IEnumerable<ConceptualOrganiserDto> conceptualOrganisers) =>
        conceptualOrganisers.Select(s => new ConceptualOrganiserModel
        {
            Name = s.Name,
            ContentDescriptions = s.ContentDescriptions.ConvertFromDtos()
        })
        .ToList();
}
