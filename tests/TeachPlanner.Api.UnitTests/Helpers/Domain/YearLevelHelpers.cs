using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Shared.Enums;

namespace TeachPlanner.Api.Tests.Helpers.Domain;

public static class YearLevelHelpers
{
    public static List<YearLevel> CreateYearLevels()
    {
        List<YearLevel> yearLevels =
        [
            YearLevel.Create(YearLevelValue.Year1, ""),
            YearLevel.Create(YearLevelValue.Year2, "")
        ];

        return yearLevels;
    }
}