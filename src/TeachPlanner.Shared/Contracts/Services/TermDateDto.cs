using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.Shared.Contracts.Services;
public record TermDateDto(
    string StartDate,
    string EndDate);

public static class TermDateDtoExtensions
{
    public static IEnumerable<TermDateDto> ToDtos(this IEnumerable<TermDate> termDates) =>
        termDates.Select(td => new TermDateDto(td.StartDate.ToString("yyyy-MM-dd"), td.EndDate.ToString("yyyy-MM-dd")));
}
