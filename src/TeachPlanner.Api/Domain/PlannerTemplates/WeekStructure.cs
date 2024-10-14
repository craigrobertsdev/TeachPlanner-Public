using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Shared.Contracts.PlannerTemplates;
using TeachPlanner.Shared.StronglyTypedIds;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.Domain.PlannerTemplates;

/// <summary>
///     This is used as a template for creating a WeekPlanner in the UI. If there are no lessons planned for a given
///     period, the WeekPlanner will use the WeekStructure to determine what to display.
///     Manages the number of periods in a week, which includes lessons and breaks.
///     A YearData object will have a single WeekStructure object.
///     period, the WeekPlanner will use the WeekStructure to determine what to display.
/// </summary>
public class WeekStructure : Entity<WeekStructureId>
{
    private readonly List<DayTemplate> _dayTemplates = [];
    private readonly List<TemplatePeriod> _periods = [];

    private WeekStructure(WeekStructureId id, List<TemplatePeriod> periods, List<DayTemplate> dayTemplates,
        TeacherId teacherId) : base(id)
    {
        _periods = periods;
        _dayTemplates = dayTemplates;
        TeacherId = teacherId;
    }

    private WeekStructure(WeekStructureId id, List<TemplatePeriod> periods, TeacherId teacherId) : base(id)
    {
        _periods = periods;
        TeacherId = teacherId;
    }

    private WeekStructure(WeekStructureId id, TeacherId teacherId) : base(id)
    {
        TeacherId = teacherId;
    }

    public TeacherId TeacherId { get; private set; }

    /// <summary>
    ///     The number of lessons and breaks in a day, ordered by their start time.
    /// </summary>
    public IReadOnlyList<TemplatePeriod> Periods => _periods.AsReadOnly();

    public int NumberOfPeriods => Periods.Count;

    /// <summary>
    ///     The planned lessons for each day. This is used to fill in the gaps in the WeekPlanner.
    ///     If the teacher has a different lesson planned for a given day, the WeekPlanner will use that instead.
    /// </summary>
    public IReadOnlyList<DayTemplate> DayTemplates => _dayTemplates.AsReadOnly();

    public void SetPeriods(IEnumerable<TemplatePeriod> periods)
    {
        _periods.Clear();
        _periods.AddRange(periods);
    }

    public void SortPeriods()
    {
        _periods.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));
    }

    public void SetDayTemplates(IReadOnlyList<DayTemplate> dayTemplates)
    {
        _dayTemplates.Clear();
        for (var i = 0; i < dayTemplates.Count; i++)
        {
            _dayTemplates.Insert(i, dayTemplates[i]);
        }
    }

    public static WeekStructure Create(List<TemplatePeriod> periods, List<DayTemplate> dayTemplates,
        TeacherId teacherId)
    {
        return new WeekStructure(new WeekStructureId(Guid.NewGuid()), periods, dayTemplates, teacherId);
    }

    public static WeekStructure Create(List<TemplatePeriod> periods, TeacherId teacherId)
    {
        return new WeekStructure(new WeekStructureId(Guid.NewGuid()), periods, teacherId);
    }

    public static WeekStructure Create(TeacherId teacherId)
    {
        return new WeekStructure(new WeekStructureId(Guid.NewGuid()), teacherId);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private WeekStructure()
    {
    }
}

public static class WeekStructureExtensions
{
    public static WeekStructureDto ToDto(this WeekStructure weekStructure)
    {
        return new WeekStructureDto(
            weekStructure.Periods.ToList(),
            weekStructure.DayTemplates.ToDtos());
    }

    public static WeekStructureDto ToDayPlanTemplateDto(this WeekStructure weekStructure)
    {
        return new WeekStructureDto(weekStructure.Periods.ToList(), weekStructure.DayTemplates.ToDtos());
    }
}