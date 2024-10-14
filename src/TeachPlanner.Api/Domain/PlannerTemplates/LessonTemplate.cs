using TeachPlanner.Shared.Contracts.PlannerTemplates;

namespace TeachPlanner.Api.Domain.PlannerTemplates;

/// <summary>
///     Represents a planner entry containing the subject and number of periods for a lesson.
/// </summary>
public record LessonTemplate
{
    public LessonTemplate(string subjectName, int numberOfPeriods, int startPeriod)
    {
        SubjectName = subjectName;
        NumberOfPeriods = numberOfPeriods;
        StartPeriod = startPeriod;
    }

    public string SubjectName { get; init; }
    public int NumberOfPeriods { get; init; }
        public int StartPeriod { get; init; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private LessonTemplate()
    {
    }
}

public static class LessonTemplateExtensions
{
    public static List<LessonTemplateDto> ToDtos(this IEnumerable<LessonTemplate> lessonTemplates)
    {
        return lessonTemplates.Select(lt => new LessonTemplateDto(lt.SubjectName, lt.NumberOfPeriods, lt.StartPeriod))
            .ToList();
    }
}