using FluentAssertions;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Tests.Helpers.Domain;
using TeachPlanner.Shared.Enums;

namespace TeachPlanner.Api.Tests.Domain;

public class CurriculumSubjectTests
{
    [Fact]
    public void RemoveYearLevelsNotTaught_WhenYearLevelHasUntaughtSubject_ShouldRemoveUntaughtYearLevels()
    {
        // Arrange
        var subject = SubjectHelpers.CreateCurriculumSubjectWithYearLevels();
        var yearLevels = new List<YearLevelValue> { YearLevelValue.Year1, YearLevelValue.Year2 };

        // Act
        var redactedYearLevels = subject.RemoveYearLevelsNotTaught(yearLevels);

        // Assert
        redactedYearLevels.Should().HaveCount(1);
        redactedYearLevels.Should().Contain(yl => yl.YearLevelValue == YearLevelValue.Year1);
    }

    [Fact]
    public void RemoveYearLevelsNotTaught_WhenYearLevelHasBandLevels_ShouldRetunMatchingYearLevel()
    {
        // Arrange
        var subject = SubjectHelpers.CreateCurriculumSubjectWithBandLevels();
        var yearLevels = new List<YearLevelValue> { YearLevelValue.Year1 };

        // Act
        var redactedYearLevels = subject.RemoveYearLevelsNotTaught(yearLevels);

        // Assert
        redactedYearLevels.Should().HaveCount(1);
        redactedYearLevels.Should().Contain(yl => yl.YearLevelValue == YearLevelValue.Years1To2);
    }

    [Fact]
    public void AddYearLevel_WhenAddingDuplicate_ShouldNotAddAnother()
    {
        // Arrange
        var subject = SubjectHelpers.CreateCurriculumSubject();

        // Act
        subject.AddYearLevel(YearLevel.Create(YearLevelValue.Year1, ""));

        // Assert
        subject.YearLevels.Count.Should().Be(2);
    }
}