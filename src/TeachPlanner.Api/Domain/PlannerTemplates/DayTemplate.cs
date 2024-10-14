using TeachPlanner.Shared.Contracts.PlannerTemplates;

namespace TeachPlanner.Api.Domain.PlannerTemplates;

/// <summary>
///     Holds the structure of a day, including the periods and their order.
///     This will be associated with many day plans and be used as the base to record
///     which subjects are being taught on a given day and how many periods they take.
///     These will be read secondarily to the actual lesson plans to fill in the gaps prior to a lesson being planned.
/// </summary>
public class DayTemplate
{
    private readonly List<LessonTemplate> _lessons = [];

    private DayTemplate(List<LessonTemplate> lessons)
    {
        _lessons = lessons;
    }

    private DayTemplate()
    {
    }

    public IReadOnlyList<LessonTemplate> Lessons => _lessons.AsReadOnly();
    public bool IsNonWorkingDay { get; private set; }

    public static DayTemplate Create(List<LessonTemplate> lessons)
    {
        return new DayTemplate(lessons);
    }
}

public static class DayTemplateExtensions
{
    public static DayTemplateDto[] ToDtos(this IEnumerable<DayTemplate> dayTemplates)
    {
        return dayTemplates.Select(dt => new DayTemplateDto(dt.Lessons.ToDtos(), dt.IsNonWorkingDay)).ToArray();
    }

    // public static DayTemplate[] FromDtos(this IEnumerable<DayTemplateDto> dayTemplates) =>
    //     dayTemplates.Select(dt => DayTemplate.Create(dt.Lessons.FromDtos())).ToArray();
}