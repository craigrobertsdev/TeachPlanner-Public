using FluentAssertions;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Api.UnitTests.Domain;
public class YearLevelTests
{
    private readonly List<YearLevel> _yearLevelsWithYearLevelValues;
    private readonly List<YearLevel> _yearLevelsWithBandLevelValues;
    public YearLevelTests()
    {
        _yearLevelsWithYearLevelValues = new() {
            YearLevel.Create(YearLevelValue.Reception, ""),
            YearLevel.Create(YearLevelValue.Year1, ""),
            YearLevel.Create(YearLevelValue.Year2, ""),
            YearLevel.Create(YearLevelValue.Year3, ""),
        };

        _yearLevelsWithBandLevelValues = new() {
            YearLevel.Create(YearLevelValue.Reception, ""),
            YearLevel.Create(YearLevelValue.Years1to2, ""),
            YearLevel.Create(YearLevelValue.Years3to4, ""),
            YearLevel.Create(YearLevelValue.Years5to6, ""),
        };
    }

    [Fact]
    public void GetFromYearLevelValue_WhenPassedYearLevelAndHasNonNullYearLevel_ShouldReturnCorrectYearLevel()
    {
        // Arrange
        var yearLevel = YearLevelValue.Year1;

        // Act
        var result = _yearLevelsWithYearLevelValues.GetFromYearLevelValue(yearLevel);

        // Assert
        result.Should().BeEquivalentTo(_yearLevelsWithYearLevelValues[1]);
    }

    [Fact]
    public void GetFromYearLevelValue_WhenPassedYearLevelAndHasBandLevel_ShouldReturnCorrectYearLevel()
    {
        // Arrange
        var yearLevel = YearLevelValue.Year1;

        // Act
        var result = _yearLevelsWithBandLevelValues.GetFromYearLevelValue(yearLevel);

        // Assert
        result.Should().BeEquivalentTo(_yearLevelsWithBandLevelValues[1]);
    }
}
