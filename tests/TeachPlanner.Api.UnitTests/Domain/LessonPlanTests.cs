using FluentAssertions;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Tests.Helpers.Domain;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Domain;

public class LessonPlanTests
{
    [Fact]
    public void AddResource_IfNotInCollection_Adds()
    {
        // Arrange
        var lessonPlan = LessonPlanHelpers.CreateLessonPlan();
        var resource = ResourceHelpers.CreateBasicResource();

        // Act
        lessonPlan.AddResource(resource);

        // Assert
        lessonPlan.Resources.Should().ContainSingle(r => r == resource);
    }

    [Fact]
    public void SetNumberOfLessons_WhenNewNumberOfLessons_ShouldUpdate()
    {
        // Arrange
        var lessonPlan = LessonPlanHelpers.CreateLessonPlan();
        var timeUpdated = lessonPlan.UpdatedDateTime;

        // Act
        lessonPlan.SetNumberOfLessons(2);

        // Assert
        lessonPlan.NumberOfLessons.Should().Be(2);
        lessonPlan.UpdatedDateTime.Should().BeAfter(timeUpdated);
    }

    [Fact]
    public void SetNumberOfLessons_WhenSameNumberOfLessons_ShouldDoNothing()
    {
        // Arrange
        var lessonPlan = LessonPlanHelpers.CreateLessonPlan();
        var timeUpdated = lessonPlan.UpdatedDateTime;

        // Act
        lessonPlan.SetNumberOfLessons(1);

        // Assert
        lessonPlan.NumberOfLessons.Should().Be(1);
        lessonPlan.UpdatedDateTime.Should().Be(timeUpdated);
    }

    [Fact]
    public void Create_WhenNumberOfLessonsOutOfRange_ShouldThrowException()
    {
        var yearDataId = new YearDataId(Guid.NewGuid());
        var subjectId = new SubjectId(Guid.NewGuid());

        // Act
        var act = () => LessonPlan.Create(yearDataId, subjectId, [], "", "", 0, 1, new DateOnly(), []);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_WhenStartPeriodOutOfRange_ShouldThrowException()
    {
        var yearDataId = new YearDataId(Guid.NewGuid());
        var subjectId = new SubjectId(Guid.NewGuid());

        // Act
        var act = () => LessonPlan.Create(yearDataId, subjectId, [], "", "", 1, 0, new DateOnly(), []);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}