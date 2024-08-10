using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.Shared.Contracts.WeekPlanners;

public record DayPlanTemplateDto(IEnumerable<PeriodDto> Pattern)
{
    public static DayPlanTemplateDto Create(WeekStructure weekStructure)
    {
        var dayPlan = new List<PeriodDto>(weekStructure.Periods.ToDto());

        return new DayPlanTemplateDto(dayPlan);
    }
}
