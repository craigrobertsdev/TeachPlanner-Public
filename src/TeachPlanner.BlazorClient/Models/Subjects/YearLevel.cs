using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Enums;

namespace TeachPlanner.BlazorClient.Models.Subjects;

public record YearLevel
{
    public YearLevelValue YearLevelValue { get; set; }
    public List<ConceptualOrganiser> ConceptualOrganisers { get; set; } = [];
}

public static class YearLevelExtensions
{
    public static List<YearLevel> ConvertFromDtos(this IEnumerable<YearLevelDto> yearLevels)
    {
        return yearLevels.Select(yl => new YearLevel
            {
                YearLevelValue = yl.YearLevel, ConceptualOrganisers = yl.ConceptualOrganisers.ConvertFromDtos()
            })
            .ToList();
    }

    public static Dictionary<YearLevelValue, List<ContentDescription>> GetContentDescriptions(
        this List<YearLevel> yearLevels)
    {
        var dict = new Dictionary<YearLevelValue, List<ContentDescription>>();
        foreach (var yearLevel in yearLevels)
        {
            var cds = yearLevel.ConceptualOrganisers.SelectMany(s => s.ContentDescriptions).ToList();
            if (cds.Count == 0)
            {
                continue;
            }

            dict.Add(yearLevel.YearLevelValue, cds);
        }

        return dict;
    }
}