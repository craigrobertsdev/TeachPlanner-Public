using TeachPlanner.Shared.Contracts.PlannerTemplates;
using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public class WeekStructureModel
{
    // Represents the lessons and breaks, in order, for each day
    public List<TemplatePeriodModel> Periods { get; set; } = [];
    public DayTemplateModel?[] DayTemplates = new DayTemplateModel[5]; // A null value represents a non-working day
}

public class DayTemplateModel
{
    public List<LessonTemplateModel> Lessons { get; set; } = [];
}

/// <summary>
/// The model for a lesson template
/// </summary>
public class LessonTemplateModel
{
    public DayOfWeek DayOfWeek { get; set; }
    public PeriodType Type { get; set; }
    public string SubjectName { get; set; } = default!;
    public int NumberOfPeriods { get; set; }
    public int StartPeriod { get; set; }
}

public static class WeekStructureModelExtensions
{
    public static WeekStructureModel ConvertFromDto(this WeekStructureDto weekStructureDto) =>
        new WeekStructureModel
        {
            Periods = weekStructureDto.Periods.ConvertFromDtos(),
            DayTemplates = weekStructureDto.DayTemplates.Select(dayTemplateDto => dayTemplateDto.ConvertFromDto()).ToArray()
        };
}

public static class DayTemplateModelExtensions
{
    public static DayTemplateModel ConvertFromDto(this DayTemplateDto dayTemplateDto) =>
        new DayTemplateModel
        {
            Lessons = dayTemplateDto.Lessons.ConvertFromDtos()
        };
}

public static class LessonTemplateModelExtensions
{
    public static LessonTemplateModel ConvertFromDto(this LessonTemplateDto lessonTemplateDto) =>
        new LessonTemplateModel
        {
            SubjectName = lessonTemplateDto.SubjectName,
            NumberOfPeriods = lessonTemplateDto.NumberOfPeriods,
            StartPeriod = lessonTemplateDto.StartPeriod
        };

    public static List<LessonTemplateModel> ConvertFromDtos(this List<LessonTemplateDto> lessonTemplateDtos) =>
        lessonTemplateDtos.Select(lessonTemplateDto => lessonTemplateDto.ConvertFromDto()).ToList();
}
