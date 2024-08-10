using TeachPlanner.Shared.Common.Interfaces.Services;

namespace TeachPlanner.Api.Features.Services;

public static class GetTermDates
{
    public static async Task<IResult> Delegate(int year, ITermDatesService termDatesService) =>
        await Task.FromResult(termDatesService.TermDatesByYear[year] is not null
        ? Results.Ok(termDatesService.TermDatesByYear[year])
        : Results.NotFound("Term dates not found"));
}
