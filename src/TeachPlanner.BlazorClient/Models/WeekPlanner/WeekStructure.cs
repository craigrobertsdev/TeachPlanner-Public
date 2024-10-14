using TeachPlanner.Shared.Contracts.PlannerTemplates;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public class WeekStructure
{
    public DayTemplate[] DayTemplates = new DayTemplate[5];

    public WeekStructure(List<TemplatePeriod> periods, DayTemplate[] dayTemplates)
    {
        Periods = periods;
        DayTemplates = dayTemplates;
    }

    // Represents the lessons and breaks, in order, for each day
    public List<TemplatePeriod> Periods { get; set; } = [];
}

public class DayTemplate
{
    public List<LessonTemplate> Lessons { get; set; } = [];
    public bool IsNonWorkingDay { get; set; }
}

/// <summary>
///     The model for a lesson template
/// </summary>
public class LessonTemplate
{
    public DayOfWeek DayOfWeek { get; set; }
    public PeriodType Type { get; set; }
    public string SubjectName { get; set; } = default!;
    public int NumberOfPeriods { get; set; }
    public int StartPeriod { get; set; }
}

public static class WeekStructureExtensions
{
    public static WeekStructure ConvertFromDto(this WeekStructureDto weekStructureDto)
    {
        return new WeekStructure(
            weekStructureDto.Periods,
            weekStructureDto.DayTemplates.Select(dayTemplateDto => dayTemplateDto.FromDto()).ToArray());
    }

    public static WeekStructureDto ToDto(this WeekStructure weekStructure)
    {
        return new WeekStructureDto(weekStructure.Periods, weekStructure.DayTemplates.ToDtos());
    }
}

public static class DayTemplateExtensions
{
    public static DayTemplate FromDto(this DayTemplateDto dayTemplateDto)
    {
        return new DayTemplate()
        {
            Lessons = dayTemplateDto.Lessons.ConvertFromDtos(), IsNonWorkingDay = dayTemplateDto.IsNonWorkingDay
        };
    }

    public static List<DayTemplate> FromDtos(this IEnumerable<DayTemplateDto> dayTemplates)
    {
        return dayTemplates.Select(dt => dt.FromDto()).ToList();
    }

    public static DayTemplateDto[] ToDtos(this IEnumerable<DayTemplate> dayTemplates)
    {
        return dayTemplates.Select(t => new DayTemplateDto(t.Lessons.ToDtos(), t.IsNonWorkingDay)).ToArray();
    }
}

public static class LessonTemplateExtensions
{
    private static LessonTemplate ConvertFromDto(this LessonTemplateDto lessonTemplateDto)
    {
        return new LessonTemplate()
        {
            SubjectName = lessonTemplateDto.SubjectName,
            NumberOfPeriods = lessonTemplateDto.NumberOfPeriods,
            StartPeriod = lessonTemplateDto.StartPeriod
        };
    }

    public static List<LessonTemplate> ConvertFromDtos(this List<LessonTemplateDto> lessonTemplateDtos)
    {
        return lessonTemplateDtos.Select(lessonTemplateDto => lessonTemplateDto.ConvertFromDto()).ToList();
    }

    public static List<LessonTemplateDto> ToDtos(this IEnumerable<LessonTemplate> templates)
    {
        return templates.Select(t => new LessonTemplateDto(t.SubjectName, t.NumberOfPeriods, t.StartPeriod)).ToList();
    }

    public static DayTemplate[] ConvertFromDtos(this IEnumerable<DayTemplateDto> dayTemplates)
    {
        return dayTemplates.Select(dt => new DayTemplate
        {
            IsNonWorkingDay = dt.IsNonWorkingDay, Lessons = dt.Lessons.ConvertFromDtos()
        }).ToArray();
    }
}