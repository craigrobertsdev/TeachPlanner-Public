using TeachPlanner.Shared.Domain.Common.Enums;

namespace TeachPlanner.Api.Features.Curriculum;

public static class GetYearLevels
{
    private static List<YearLevelValue> YearLevels = [
        YearLevelValue.Year1,
        YearLevelValue.Year2,
        YearLevelValue.Year3,
        YearLevelValue.Year4,
        YearLevelValue.Year5,
        YearLevelValue.Year6,
        YearLevelValue.Year7,
        YearLevelValue.Year8,
        YearLevelValue.Year9,
        YearLevelValue.Year10
        ];

    public static async Task<IResult> Delegate()
    {
        await Task.CompletedTask;
        return Results.Ok(YearLevels);
    }
}
