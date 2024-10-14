using TeachPlanner.Api.Domain.Common.Interfaces;
using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Shared.Contracts.PlannerTemplates;
using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.WeekPlanners;

/// <summary>
///     Represents a week of planning for a teacher.
///     This element is
/// </summary>
public sealed class WeekPlanner : Entity<WeekPlannerId>, IAggregateRoot
{
    private readonly List<DayPlan> _dayPlans = [];

    private WeekPlanner(
        WeekPlannerId id,
        YearDataId yearDataId,
        int weekNumber,
        int termNumber,
        int year,
        DateOnly weekStart) : base(id)
    {
        YearDataId = yearDataId;
        WeekStart = weekStart;
        WeekNumber = weekNumber;
        TermNumber = termNumber;
        Year = year;
        CreatedDateTime = DateTime.UtcNow;
        UpdatedDateTime = DateTime.UtcNow;

        for (var i = 0; i < 5; i++)
        {
            _dayPlans.Add(DayPlan.Create(weekStart.AddDays(i), id, [], []));
        }
    }

    public YearDataId YearDataId { get; private set; }
    public DateOnly WeekStart { get; private set; }
    public int WeekNumber { get; private set; }
    public int TermNumber { get; private set; }
    public int Year { get; private set; }
    public IReadOnlyList<DayPlan> DayPlans => SortedDayPlans.Value.AsReadOnly();
    private Lazy<List<DayPlan>> SortedDayPlans { get; set; } = new();
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    public void UpdateDayPlan(DayPlan dayPlan)
    {
        var idx = (int)dayPlan.DayOfWeek - 1;
        if (idx < 0 || idx >= 5)
        {
            throw new InvalidOperationException("DayPlan's date does not match this WeekPlanner.");
        }

        _dayPlans[idx] = dayPlan;
        SortedDayPlans = new Lazy<List<DayPlan>>(() => _dayPlans);
    }

    public static WeekPlanner Create(
        YearDataId yearDataId,
        int weekNumber,
        int termNumber,
        int year,
        DateOnly weekStart)
    {
        return new WeekPlanner(
            new WeekPlannerId(Guid.NewGuid()),
            yearDataId,
            weekNumber,
            termNumber,
            year,
            weekStart);
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private WeekPlanner()
    {
        SortedDayPlans = new Lazy<List<DayPlan>>(EnsureSorted);
        return;

        List<DayPlan> EnsureSorted()
        {
            _dayPlans.Sort((a, b) => a.DayOfWeek.CompareTo(b.DayOfWeek));
            return _dayPlans;
        }
    }
}

public static class PlannerTemplateExtensions
{
    public static List<DayTemplate> FromDtos(this IEnumerable<DayTemplateDto> dayTemplates)
    {
        return dayTemplates.Select(d => DayTemplate.Create(d.Lessons.FromDtos())).ToList();
    }

    public static List<LessonTemplate> FromDtos(this IEnumerable<LessonTemplateDto> lessonTemplates)
    {
        return lessonTemplates.Select(l => new LessonTemplate(l.SubjectName, l.NumberOfPeriods, l.StartPeriod))
            .ToList();
    }

    public static WeekStructure FromDto(this WeekStructureDto weekStructure, TeacherId teacherId)
    {
        return WeekStructure.Create(weekStructure.Periods, weekStructure.DayTemplates.FromDtos(), teacherId);
    }

    public static WeekPlannerDto ToDto(this WeekPlanner weekPlanner, WeekStructure weekStructure, List<CurriculumSubject> subjects, List<Resource> resources) =>
        new WeekPlannerDto(
            weekPlanner.DayPlans.ToDtos(subjects, resources),
            weekStructure.ToDto(),
            weekPlanner.WeekStart,
            weekPlanner.WeekNumber
        );
}