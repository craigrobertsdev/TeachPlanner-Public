namespace TeachPlanner.Shared.Contracts.Teachers.AccountSetup;

public record DayPlanPatternDto(
    List<DayPlanLessonTemplateDto> LessonTemplates,
    List<DayPlanBreakTemplateDto> BreakTemplates
    );

public record DayPlanLessonTemplateDto(
    TimeOnly StartTime,
    TimeOnly EndTime);

public record DayPlanBreakTemplateDto(
    string Name,
    TimeOnly StartTime,
    TimeOnly EndTime);
