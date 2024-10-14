using TeachPlanner.Api.Interfaces.Services;

namespace TeachPlanner.Api.Features.Services;

public static class GetTermDates
{
    public static async Task<IResult> Endpoint(int year, ITermDatesService termDatesService)
    {
        return await Task.FromResult(termDatesService.TermDatesByYear[year] is not null
            ? Results.Ok(termDatesService.TermDatesByYear[year])
            : Results.NotFound("Term dates not found"));
    }
}