using FluentAssertions;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.TermPlanners;
using TeachPlanner.Api.Tests.Helpers.Domain;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.TermPlanners;

public class TermPlannerTests
{
    [Fact]
    public void Create_OnValidInput_ShouldReturnTermPlanner()
    {
        // Arrange

        // Act
        var termPlanner = TermPlanner.Create(new YearDataId(Guid.NewGuid()), 2023,
            [YearLevelValue.Reception, YearLevelValue.Year1]);

        // Assert
        termPlanner.Should().BeOfType<TermPlanner>();
        termPlanner.CalendarYear.Should().Be(2023);
        termPlanner.YearLevels.Count.Should().Be(2);
        termPlanner.YearLevels[0].Should().Be(YearLevelValue.Reception);
    }

    [Fact]
    public void Create_OnCreating_YearLevelsShouldBeOrdered()
    {
        // Arrange
        var termPlanner = TermPlanner.Create(new YearDataId(Guid.NewGuid()), 2023,
            [YearLevelValue.Year5, YearLevelValue.Year1]);

        // Act

        // Assert
        termPlanner.YearLevels[0].Should().Be(YearLevelValue.Year1);
        termPlanner.YearLevels[1].Should().Be(YearLevelValue.Year5);
    }

    [Fact]
    public void AddYearLevel_OnAddingYearLevel_ShouldBeAdded()
    {
        // Arrange
        var termPlanner = TermPlanner.Create(new YearDataId(Guid.NewGuid()), 2023,
            [YearLevelValue.Year1]);

        // Act
        termPlanner.AddYearLevel(YearLevelValue.Year2);

        // Assert
        termPlanner.YearLevels.Should().HaveCount(2);
        termPlanner.YearLevels.Should().Contain(YearLevelValue.Year1);
        termPlanner.YearLevels.Should().Contain(YearLevelValue.Year2);
    }

    [Fact]
    public void AddYearLevel_OnAddingYearLevel_ShouldBeOrdered()
    {
        // Arrange
        var termPlanner = TermPlanner.Create(new YearDataId(Guid.NewGuid()), 2023,
            [YearLevelValue.Year5]);

        // Act
        termPlanner.AddYearLevel(YearLevelValue.Year1);

        // Assert
        termPlanner.YearLevels[0].Should().Be(YearLevelValue.Year1);
        termPlanner.YearLevels[1].Should().Be(YearLevelValue.Year5);
    }

    [Fact]
    public void AddTermPlan_OnAddingTermPlan_ShouldBeAdded()
    {
        // Arrange
        var termPlanner = TermPlanner.Create(new YearDataId(Guid.NewGuid()), 2023,
            [YearLevelValue.Reception, YearLevelValue.Year1]);
        var termPlan = TermPlan.Create(termPlanner, 1,
            [TermPlannerHelpers.CreateSubject("English", "ENG001")]);

        // Act
        termPlanner.AddTermPlan(termPlan);

        // Assert
        termPlanner.TermPlans.Should().HaveCount(1);
        termPlanner.TermPlans.Should().Contain(termPlan);
    }

    [Fact]
    public void AddTermPlan_OnAddingDuplicateTermPlan_ShouldNotBeAdded()
    {
        // Arrange
        var termPlanner = TermPlanner.Create(new YearDataId(Guid.NewGuid()), 2023,
            [YearLevelValue.Reception, YearLevelValue.Year1]);
        var termPlan = TermPlan.Create(termPlanner, 1,
            [TermPlannerHelpers.CreateSubject("English", "ENG001")]);

        // Act
        termPlanner.AddTermPlan(termPlan);
        var act = () => termPlanner.AddTermPlan(termPlan);

        // Assert
        act.Should().Throw<DuplicateTermPlanException>();
        termPlanner.TermPlans.Should().HaveCount(1);
    }

    [Fact]
    public void AddTermPlan_OnAddingFifthTermPlan_ShouldNotBeAdded()
    {
        // Arrange
        var termPlanner = TermPlannerHelpers.CreateTermPlanner();
        List<TermPlan> termPlans =
        [
            TermPlan.Create(termPlanner, 1,
                [TermPlannerHelpers.CreateSubject("English", "ENG001")]),

            TermPlan.Create(termPlanner, 2,
                [TermPlannerHelpers.CreateSubject("English", "ENG002")]),

            TermPlan.Create(termPlanner, 3,
                [TermPlannerHelpers.CreateSubject("English", "ENG003")]),

            TermPlan.Create(termPlanner, 4,
                [TermPlannerHelpers.CreateSubject("English", "ENG004")])
        ];

        foreach (var termPlan in termPlans)
        {
            termPlanner.AddTermPlan(termPlan);
        }

        // Act
        var act = () => termPlanner.AddTermPlan(TermPlan.Create(termPlanner, 4,
            [TermPlannerHelpers.CreateSubject("English", "ENG005")]));

        // Assert
        act.Should().Throw<TooManyTermPlansException>();
        termPlanner.TermPlans.Should().HaveCount(4);
    }

    [Fact]
    public void AddTermPlan_OnAddingDuplicateTermNumber_ShouldNotBeAdded()
    {
        // Arrange
        var termPlanner = TermPlannerHelpers.CreateTermPlanner();
        var termPlan = TermPlan.Create(termPlanner, 1,
            [TermPlannerHelpers.CreateSubject("English", "ENG001")]);

        // Act
        termPlanner.AddTermPlan(termPlan);
        var act = () => termPlanner.AddTermPlan(TermPlan.Create(termPlanner, 1,
            [TermPlannerHelpers.CreateSubject("English", "ENG005")]));

        // Assert
        termPlanner.TermPlans.Should().HaveCount(1);
        termPlanner.TermPlans[0].Should().BeEquivalentTo(termPlan);
        act.Should().Throw<DuplicateTermNumberException>();
    }
}