using TeachPlanner.Shared.Contracts.PlannerTemplates;
using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Domain.Common.Primatives;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Shared.Domain.PlannerTemplates;

/// <summary>
/// Manages the number of periods in a week, which includes lessons and breaks.
/// A YearData object will have a single WeekStructure object.
/// </summary>
public class WeekStructure : Entity<WeekStructureId>
{
    private readonly List<TemplatePeriod> _periods = [];
    public TeacherId TeacherId { get; private set; }
    public IReadOnlyList<TemplatePeriod> Periods => _periods.AsReadOnly();
    public int NumberOfPeriods => Periods.Count;
    private List<DayTemplate> _dayTemplates = []; // a null value indicates no template for that day, ie. a non-working day.
    public IReadOnlyList<DayTemplate> DayTemplates => _dayTemplates.AsReadOnly();

    public void SetPeriods(IEnumerable<TemplatePeriod> periods)
    {
        _periods.Clear();
        _periods.AddRange(periods);
    }

    public void AddDayTemplate(DayTemplate dayTemplate, int index)
    {
        if (_dayTemplates.Count > index)
        {
            _dayTemplates[index] = dayTemplate;
        }
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

    public static WeekStructure Create(List<TemplatePeriod> periods, TeacherId teacherId)
    {
        return new WeekStructure(new WeekStructureId(Guid.NewGuid()), periods, teacherId);
    }

    public static WeekStructure Create(TeacherId teacherId)
    {
        return new WeekStructure(new WeekStructureId(Guid.NewGuid()), teacherId);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private WeekStructure() { }
}

public static class WeekStructureExtensions
{
    public static WeekStructureDto ToDto(this WeekStructure weekStructure) =>
        new WeekStructureDto(
            weekStructure.Periods.ToDto(),
            weekStructure.DayTemplates.ToDtos());
    public static DayPlanTemplateDto ToDayPlanTemplateDto(this WeekStructure weekStructure) =>
        new DayPlanTemplateDto(weekStructure.Periods.ToDto());
}