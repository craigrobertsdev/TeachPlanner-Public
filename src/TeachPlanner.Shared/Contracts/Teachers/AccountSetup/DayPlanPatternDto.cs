namespace TeachPlanner.Shared.Contracts.Teachers.AccountSetup;

// these have been removed from use in the account setup process in favour of a weekstructuredto.
// work out whether these need to exist at all, or whether I can get away with just having a single object of type
// TemplatePeriodDto instead
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