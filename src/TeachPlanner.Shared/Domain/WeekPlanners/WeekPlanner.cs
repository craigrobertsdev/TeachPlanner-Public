using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Domain.Common.Interfaces;
using TeachPlanner.Shared.Domain.Common.Primatives;
using TeachPlanner.Shared.Domain.YearDataRecords;

namespace TeachPlanner.Shared.Domain.WeekPlanners;

public sealed class WeekPlanner : Entity<WeekPlannerId>, IAggregateRoot
{
    private readonly List<DayPlan> _dayPlans = [];
    public YearDataId YearDataId { get; private set; }
    public DateOnly WeekStart { get; private set; }
    public int WeekNumber { get; private set; }
    public int TermNumber { get; private set; }
    public int Year { get; private set; }
    public IReadOnlyList<DayPlan> DayPlans => SortedDayPlans.Value.AsReadOnly();
    private Lazy<List<DayPlan>> SortedDayPlans { get; set; } = new();
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }

    public void AddDayPlan(DayPlan dayPlan)
    {
        if (_dayPlans.Count >= 5)
        {
            throw new TooManyDayPlansInWeekPlannerException();
        }

        var idx = dayPlan.Date.DayNumber - WeekStart.DayNumber;
        if (idx < 0 || idx > 5)
        {
            throw new InvalidOperationException("DayPlan's date does not match this WeekPlanner.");
        }

        _dayPlans.Add(dayPlan);
        SortedDayPlans = new(() => _dayPlans);
    }

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
        List<DayPlan> ensureSorted()
        {
            _dayPlans.Sort((a, b) => a.Date.CompareTo(b.Date));
            return _dayPlans;
        }
        SortedDayPlans = new(ensureSorted);
    }
}