using TeachPlanner.Shared.Contracts.PlannerTemplates;

namespace TeachPlanner.Shared.Domain.PlannerTemplates;

/// <summary>
/// Holds the structure of a day, including the periods and their order.
/// This will be associated with many day plans and be used as the base to record 
/// which subjects are being taught on a given day and how many periods they take.
/// These will be read secondarily to the actual lesson plans to fill in the gaps prior to a lesson being planned.
/// </summary>
public class DayTemplate
{
    private readonly List<LessonTemplate> _lessons = [];
    public IReadOnlyList<LessonTemplate> Lessons => _lessons.AsReadOnly();

    private DayTemplate(List<LessonTemplate> lessons)
    {
        _lessons = lessons;
    }

    public static DayTemplate Create(List<LessonTemplate> lessons)
    {
        return new DayTemplate(lessons);
    }

    private DayTemplate() { }
}

public static class DayTemplateExtensions
{
    public static DayTemplateDto[] ToDtos(this IEnumerable<DayTemplate> dayTemplates) =>
        dayTemplates.Select(dt => new DayTemplateDto(dt.Lessons.ToDtos())).ToArray();
}
