using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.IntegrationTests.WeekPlanners;

public class GetWeekPlannerIntegrationTests : IClassFixture<MySqlFixture>
{
    private readonly MySqlFixture _fixture;
    private readonly Teacher _teacher;
    private readonly int _calendarYear = 2024;
    private readonly YearDataId _yearDataId;
    private readonly SubjectId _subjectId;

    public GetWeekPlannerIntegrationTests(MySqlFixture fixture)
    {
        _fixture = fixture;
        var dbContext = fixture.CreateDbContext();
        _teacher = dbContext.Teachers.First();
        _yearDataId = dbContext.YearData.First().Id;
        _subjectId = dbContext.CurriculumSubjects.First().Id;    
    }
    
    [Fact]
    public async Task ReturnsNullIfWeekPlannerNotFound()
    {

        // Assert
        // result.Should().BeNull();
    }

    [Fact]
    public async Task ReturnsWeekPlannerIfExists()
    {
        
        // result.Should().NotBeNull();
        // result.Should().BeOfType<WeekPlannerDto>();
        // result.WeekNumber.Should().Be(1);
        // result.WeekStructure.DayTemplates[0].Lessons.Count.Should().Be(2);
        // result.WeekStructure.DayTemplates[0].Lessons[1].NumberOfPeriods.Should().Be(2);
    }
}