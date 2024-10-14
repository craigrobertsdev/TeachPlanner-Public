using FluentAssertions;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Tests.Helpers.Domain;
using TeachPlanner.Shared.Enums;

namespace TeachPlanner.Api.Tests.Domain;

public class YearLevelTests
{
    private readonly List<YearLevel> _yearLevelsWithBandLevelValues;
    private readonly List<YearLevel> _yearLevelsWithYearLevelValues;

    public YearLevelTests()
    {
        _yearLevelsWithYearLevelValues =
        [
            YearLevel.Create(YearLevelValue.Reception, ""),
            YearLevel.Create(YearLevelValue.Year1, ""),
            YearLevel.Create(YearLevelValue.Year2, ""),
            YearLevel.Create(YearLevelValue.Year3, "")
        ];

        _yearLevelsWithBandLevelValues =
        [
            YearLevel.Create(YearLevelValue.Reception, ""),
            YearLevel.Create(YearLevelValue.Years1To2, ""),
            YearLevel.Create(YearLevelValue.Years3To4, ""),
            YearLevel.Create(YearLevelValue.Years5To6, "")
        ];
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

    [Fact]
    public void GetFromYearLevelValue_WhenYearLevelInCollection_ShouldReturnYearLevel()
    {
        // Arrange
        var yearLevels = YearLevelHelpers.CreateYearLevels();

        // Act
        var yearLevel = yearLevels.GetFromYearLevelValue(YearLevelValue.Year1);

        // Assert
        yearLevel.Should().NotBeNull();
        yearLevel!.YearLevelValue.Should().Be(YearLevelValue.Year1);
    }

    [Fact]
    public void GetFromYearLevelValue_WhenYearLevelNotInCollection_ShouldReturnNull()
    {
        // Arrange
        var yearLevels = YearLevelHelpers.CreateYearLevels();

        // Act
        var yearLevel = yearLevels.GetFromYearLevelValue(YearLevelValue.Year5);

        // Assert
        yearLevel.Should().BeNull();
    }
}