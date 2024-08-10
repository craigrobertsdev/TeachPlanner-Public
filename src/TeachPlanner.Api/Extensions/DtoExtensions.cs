using TeachPlanner.Shared.Contracts.LessonPlans;
using TeachPlanner.Shared.Contracts.Resources;
using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Domain.Common.Planner;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.WeekPlanners;

namespace TeachPlanner.Api.Extensions;

public static class DtoExtensions
{
    public static List<DayPlanDto> ToDtos(this IEnumerable<DayPlan> dayPlans, IEnumerable<Resource> resources, IEnumerable<CurriculumSubject> subjects) =>
        dayPlans.Select(dp => new DayPlanDto(
            dp.Date,
            dp.LessonPlans.ToDtos(resources, subjects),
            dp.SchoolEvents.ToDtos()))
        .ToList();


    public static List<SchoolEventDto> ToDtos(this IEnumerable<SchoolEvent> events) =>
        events.Select(e => new SchoolEventDto(
            e.Location,
            e.Name,
            e.FullDay,
            e.EventStart,
            e.EventEnd))
        .ToList();

    public static List<ResourceDto> ToDtos(this IEnumerable<Resource> resources) =>
        resources.Select(r => new ResourceDto(
            r.Id,
            r.Name,
            r.Url,
            r.IsAssessment,
            r.YearLevels))
        .ToList();
}
