using FluentAssertions;
using TeachPlanner.Api.UnitTests.Helpers;

namespace TeachPlanner.Api.UnitTests.Domain;
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
}
