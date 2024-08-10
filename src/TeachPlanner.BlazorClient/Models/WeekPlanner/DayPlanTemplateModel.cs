using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public record DayPlanTemplateModel(List<TemplatePeriodModel> Periods);

/// <summary>
/// The model for any entry in a day plan. Used for creating the structure of the template.
/// </summary>
public class TemplatePeriodModel(PeriodType type, string? name, TimeOnly startTime, TimeOnly endTime)
{
    public PeriodType Type { get; set; } = type;
    public string? Name { get; set; } = name;
    public TimeOnly StartTime { get; set; } = startTime;
    public TimeOnly EndTime { get; set; } = endTime;

}

public static class DayPlanTemplateModelExtensions
{
    public static DayPlanTemplateModel ConvertFromDto(this DayPlanTemplateDto weekStructureDto) =>
        new DayPlanTemplateModel(weekStructureDto.Pattern.ConvertFromDtos());

    public static List<TemplatePeriodModel> ConvertFromDtos(this IEnumerable<PeriodDto> periods) =>
        periods.Select(p => new TemplatePeriodModel(Enum.Parse<PeriodType>(p.PeriodType), p.Name, p.StartTime, p.EndTime))
        .ToList();
}
