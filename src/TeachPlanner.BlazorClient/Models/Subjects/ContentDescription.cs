using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Enums;

namespace TeachPlanner.BlazorClient.Models.Subjects;

public record ContentDescription
{
    public Guid Id { get; set; }
    public YearLevelValue YearLevel { get; set; }
    public string Topic { get; set; } = string.Empty;
    public List<string> CurriculumCodes { get; set; } = [];
    public string Text { get; set; } = string.Empty;
    public bool AlreadyPlannedFor { get; set; }
}

public static class ContentDescriptionExtensions
{
    public static List<ContentDescription> ConvertFromDtos(this IEnumerable<ContentDescriptionDto> contentDescriptions)
    {
        return contentDescriptions.Select(cd => new ContentDescription
            {
                Id = cd.Id,
                Topic = cd.Topic,
                CurriculumCodes = cd.CurriculumCodes.ToList(),
                Text = cd.ContentDescription
            })
            .ToList();
    }

    public static Dictionary<YearLevelValue, List<ContentDescription>> ConvertFromDtos(
        this Dictionary<YearLevelValue, List<ContentDescriptionDto>> contentDescriptions)
    {
        var yearLevelContentDescriptions = new Dictionary<YearLevelValue, List<ContentDescription>>();
        foreach (var key in contentDescriptions.Keys)
        {
            yearLevelContentDescriptions.Add(
                key,
                contentDescriptions[key].ConvertFromDtos());
        }

        return yearLevelContentDescriptions;
    }
}