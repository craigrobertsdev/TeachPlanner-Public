using TeachPlanner.Shared.Contracts.PlannerTemplates;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Shared.Domain.PlannerTemplates;
/// <summary>
/// Represents a planner entry containing the subject and number of periods for a lesson.
/// </summary>
public record LessonTemplate
{
    public string SubjectName { get; private set; }
    public int NumberOfPeriods { get; private set; }
    public int StartPeriod { get; private set; }

    public LessonTemplate(string subjectName, int numberOfPeriods, int startPeriod)
    {
        SubjectName = subjectName;
        NumberOfPeriods = numberOfPeriods;
        StartPeriod = startPeriod;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private LessonTemplate() { }
}

public static class LessonTemplateExtensions
{
    public static List<LessonTemplateDto> ToDtos(this IEnumerable<LessonTemplate> lessonTemplates) =>
        lessonTemplates.Select(lt => new LessonTemplateDto(lt.SubjectName, lt.NumberOfPeriods, lt.StartPeriod)).ToList();
}
