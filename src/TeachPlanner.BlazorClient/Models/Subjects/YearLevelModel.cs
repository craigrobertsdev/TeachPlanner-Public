using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Domain.Common.Enums;

namespace TeachPlanner.BlazorClient.Models.Subjects;

public record YearLevelModel
{
    public YearLevelValue YearLevel { get; set; }
    public List<ConceptualOrganiserModel> ConceptualOrganisers { get; set; } = [];
}

public static class YearLevelModelExtensions
{
    public static List<YearLevelModel> ConvertFromDtos(this IEnumerable<YearLevelDto> yearLevels) =>
        yearLevels.Select(yl => new YearLevelModel
        {
            YearLevel = yl.YearLevel,
            ConceptualOrganisers = yl.ConceptualOrganisers.ConvertFromDtos()
        })
        .ToList();

    public static Dictionary<YearLevelValue, List<ContentDescriptionModel>> GetContentDescriptions(this List<YearLevelModel> yearLevels)
    {
        var dict = new Dictionary<YearLevelValue, List<ContentDescriptionModel>>();
        foreach (var yearLevel in yearLevels)
        {
            var cds = yearLevel.ConceptualOrganisers.SelectMany(s => s.ContentDescriptions).ToList();
            if (cds.Count == 0)
            {
                continue;
            }

            dict.Add(yearLevel.YearLevel, cds);
        }

        return dict;
    }
}
