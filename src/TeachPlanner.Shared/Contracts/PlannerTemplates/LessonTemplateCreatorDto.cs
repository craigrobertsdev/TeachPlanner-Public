using TeachPlanner.Shared.Contracts.WeekPlanners;

namespace TeachPlanner.Shared.Contracts.PlannerTemplates;
public record LessonTemplateCreatorDto(WeekStructureDto WeekStructure, List<string> Subjects);

public record WeekStructureDto(List<PeriodDto> Periods, DayTemplateDto[] DayTemplates);

public record DayTemplateDto(List<LessonTemplateDto> Lessons);

public record LessonTemplateDto(string SubjectName, int NumberOfPeriods, int StartPeriod);
