using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TeachPlanner.Shared.Enums;

namespace TeachPlanner.Api.IntegrationTests.Features.Subjects;

public class GetYearLevelsTests(MySqlFixture fixture) : IClassFixture<MySqlFixture>
{
    
    [Fact]
    public async Task ShouldReturnYearLevels1To10()
    {
        // Arrange
        var expectedYearLevels = new List<YearLevelValue>
        {
            YearLevelValue.Reception,
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
        };
        
        var client = fixture.CreateClient();
        var dbContext = fixture.CreateDbContext();
        var teacher = dbContext.Teachers.First();

        // Act
        var result = await client.GetAsync($"/api/{teacher.Id.Value}/curriculum/yearLevels");
        var yearLevels = await result.Content.ReadFromJsonAsync<List<YearLevelValue>>();
        
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        yearLevels.Should().BeEquivalentTo(expectedYearLevels);
    }

}