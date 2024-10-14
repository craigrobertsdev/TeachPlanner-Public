using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Shared.Contracts.PlannerTemplates;

public record LessonTemplateCreatorDto(WeekStructureDto WeekStructure, List<string> Subjects);

public record WeekStructureDto(List<TemplatePeriod> Periods, DayTemplateDto[] DayTemplates);

public record DayTemplateDto(List<LessonTemplateDto> Lessons, bool IsNonWorkingDay);

public record LessonTemplateDto(string SubjectName, int NumberOfPeriods, int StartPeriod);