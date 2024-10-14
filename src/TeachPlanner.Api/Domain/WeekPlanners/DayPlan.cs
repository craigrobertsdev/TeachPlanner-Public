using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Extensions;
using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.StronglyTypedIds;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.Domain.WeekPlanners;

/// <summary>
///     Represents the actually planned lessons and events for a given day.
/// </summary>
public class DayPlan : Entity<DayPlanId>
{
    private readonly List<LessonPlan> _lessonPlans = [];
    private readonly List<SchoolEvent> _schoolEvents = [];

    private DayPlan(DayPlanId id, WeekPlannerId weekPlannerId, DateOnly date, List<LessonPlan> lessonPlans,
        List<SchoolEvent>? schoolEvents) :
        base(id)
    {
        _lessonPlans = lessonPlans;
        WeekPlannerId = weekPlannerId;
        Date = date;

        if (schoolEvents is not null)
        {
            _schoolEvents = schoolEvents;
        }
    }

    public WeekPlannerId WeekPlannerId { get; private set; }
    public DateOnly Date { get; private set; }
    public DayOfWeek DayOfWeek => Date.DayOfWeek;
    public IReadOnlyList<LessonPlan> LessonPlans => _lessonPlans.AsReadOnly();
    public IReadOnlyList<SchoolEvent> SchoolEvents => _schoolEvents.AsReadOnly();

    public void AddLessonPlan(LessonPlan lessonPlan)
    {
        if (_lessonPlans.Contains(lessonPlan))
        {
            return;
        }

        _lessonPlans.Add(lessonPlan);
    }

    public static DayPlan Create(DateOnly date, WeekPlannerId weekPlannerId, List<LessonPlan> lessonPlans,
        List<SchoolEvent> schoolEvents)
    {
        return new DayPlan(new DayPlanId(Guid.NewGuid()), weekPlannerId, date, lessonPlans, schoolEvents);
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private DayPlan()
    {
    }
}

public static class DayPlanExtensions
{
    public static List<DayPlanDto> ToDtos(this IEnumerable<DayPlan> dayPlans, List<CurriculumSubject> subjects, List<Resource> resources) =>
        dayPlans.Select(dp => new DayPlanDto(dp.Date, dp.LessonPlans.ToDtos(resources, subjects), dp.SchoolEvents.ToDtos())).ToList();
}