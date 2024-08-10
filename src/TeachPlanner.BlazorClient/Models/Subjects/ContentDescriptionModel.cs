using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Domain.Common.Enums;

namespace TeachPlanner.BlazorClient.Models.Subjects;

public record ContentDescriptionModel
{
    public Guid Id { get; set; }
    public YearLevelValue YearLevel { get; set; }
    public string Topic { get; set; } = string.Empty;
    public List<string> CurriculumCodes { get; set; } = [];
    public string Text { get; set; } = string.Empty;
    public bool AlreadyPlannedFor { get; set; }
}

public static class ContentDescriptionModelExtensions
{
    public static List<ContentDescriptionModel> ConvertFromDtos(this IEnumerable<ContentDescriptionDto> contentDescriptions) =>
        contentDescriptions.Select(cd => new ContentDescriptionModel
        {
            Id = cd.Id,
            Topic = cd.Topic,
            CurriculumCodes = cd.CurriculumCodes.ToList(),
            Text = cd.ContentDescription
        })
        .ToList();

    public static Dictionary<YearLevelValue, List<ContentDescriptionModel>> ConvertFromDtos(this Dictionary<YearLevelValue, List<ContentDescriptionDto>> contentDescriptions)
    {
        var yearLevelContentDescriptions = new Dictionary<YearLevelValue, List<ContentDescriptionModel>>();
        foreach (var key in contentDescriptions.Keys)
        {
            yearLevelContentDescriptions.Add(
                key,
                contentDescriptions[key].ConvertFromDtos());
        }

        return yearLevelContentDescriptions;
    }
}
