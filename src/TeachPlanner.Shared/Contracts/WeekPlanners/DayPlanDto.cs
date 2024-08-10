using TeachPlanner.Shared.Contracts.LessonPlans;

namespace TeachPlanner.Shared.Contracts.WeekPlanners;
public record DayPlanDto(DateOnly Date, List<LessonPlanDto> LessonPlans, List<SchoolEventDto> SchoolEvents);
