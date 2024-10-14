using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.IntegrationTests.Helpers;
using TeachPlanner.Shared.Contracts.WeekPlanners;

namespace TeachPlanner.Api.IntegrationTests.Features.WeekPlanners;

public class CreateWeekPlannerTests : IClassFixture<MySqlFixture>
{
    private readonly MySqlFixture _fixture;
    private readonly HttpClient _client;

    public CreateWeekPlannerTests(MySqlFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }
    
    [Theory]
    [MemberData(nameof(TermStartDateData))]
    public async Task WhenNoWeekPlannerExists_ShouldCreateWeekPlanner(int weekNumber, DateOnly weekStart)
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var teacher = dbContext.Teachers.First();
        var request = new CreateWeekPlannerRequest(weekNumber, 1, 2024);

        // Act
        var result = await _client.PostAsJsonAsync($"api/{teacher.Id.Value}/week-planner", request);

        // Assert
        result.Should().NotBeNull();

        var data = await result.Content.ReadFromJsonAsync<WeekPlannerDto>();
        data.Should().NotBeNull();
        data!.WeekNumber.Should().Be(request.WeekNumber);
        data.WeekStart.Should().Be(weekStart);
        data.WeekStructure.Should().NotBeNull();
        data.WeekStructure.DayTemplates.Length.Should().Be(1);
    }

    [Fact]
    public async Task WhenWeekPlannerExists_ShouldNotOverwriteExisting()
    {
        // Arrange
        var dbContext = _fixture.CreateDbContext();
        var teacher = dbContext.Teachers.First();
        var existingWeekPlanner = dbContext.WeekPlanners
            .Include(wp => wp.DayPlans)
            .ThenInclude(db => db.LessonPlans)
            .First();
        var subjectId = dbContext.CurriculumSubjects.First().Id;
        var yearDataId = dbContext.YearData.First().Id;
        
        var lessonPlan = LessonPlan.Create(yearDataId, subjectId, [], "", "", 1, 1,
            TestConstants.FirstDayOfTerm2024, []);
        existingWeekPlanner.DayPlans[0].AddLessonPlan(lessonPlan);
        dbContext.WeekPlanners.Update(existingWeekPlanner);
        await dbContext.SaveChangesAsync();
        
        var request = new CreateWeekPlannerRequest(1, 1, 2024);

        // Act
        var result = await _client.PostAsJsonAsync($"api/{teacher.Id.Value}/week-planner", request);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var context = _fixture.CreateDbContext();
        var weekPlanner = context.WeekPlanners
            .Include(wp => wp.DayPlans)
            .ThenInclude(dp => dp.LessonPlans)
            .First();
        
        weekPlanner.DayPlans[0].LessonPlans.Should().ContainSingle();
    }
    
    public static IEnumerable<object[]> TermStartDateData()
    {
        yield return [2, TestConstants.FirstDayOfTerm2024.AddDays(7)];
        yield return [3, TestConstants.FirstDayOfTerm2024.AddDays(14)];
    }
}