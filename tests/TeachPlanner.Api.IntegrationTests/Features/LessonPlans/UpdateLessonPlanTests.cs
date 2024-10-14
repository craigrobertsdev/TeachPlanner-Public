using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.IntegrationTests.Helpers;
using TeachPlanner.Shared.Contracts.LessonPlans;
using static TeachPlanner.Api.IntegrationTests.Helpers.WeekPlannerHelpers;

namespace TeachPlanner.Api.IntegrationTests.Features.LessonPlans;

public class UpdateLessonPlanTests : IClassFixture<MySqlFixture>
{

    private readonly MySqlFixture _fixture;
    private readonly Teacher _teacher;
    private readonly HttpClient _client;
    private readonly CurriculumSubject _subject;

    public UpdateLessonPlanTests(MySqlFixture fixture)
    {
        _fixture = fixture;
        using var dbContext = fixture.CreateDbContext();
        _teacher = dbContext.Teachers.First();
        _client = _fixture.CreateClient();
        _subject = dbContext.CurriculumSubjects.First();
    }
    
    [Fact]
    public async Task LessonPlanAlreadyExists_ShouldUpdateInstead()
    {
        // Arrange
        var weekNumber = 1;
        var lessonDate = TestConstants.FirstDayOfTerm2024.AddDays((weekNumber - 1) * 7);
        await AddWeekPlannerToDatabase(_fixture, _teacher, lessonDate, weekNumber);
        await using var dbContext = _fixture.CreateDbContext();
        var newLessonPlan = dbContext.LessonPlans.First();
        var lessonPlanRequest = new UpdateLessonPlanRequest(
            newLessonPlan.Id.Value,
            _subject.Id.Value,
            [],
            "",
            "",
            [],
            lessonDate,
            2,
            1);

        // Act
        var result = await _client.PatchAsJsonAsync($"/api/{_teacher.Id.Value}/lesson-plans", lessonPlanRequest);

        // Assert
        var newDbContext = _fixture.CreateDbContext();
        var lessonPlan = await newDbContext.LessonPlans.FirstAsync();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        lessonPlan.Should().NotBeNull();
        lessonPlan.NumberOfLessons.Should().Be(2);
    }

    [Fact]
    public async Task WhenChangeToNumberOfLessonsCausesOverlapWithExistingLessonPlan_DeletesOverlappedLessonPlansAndUpdates()
    {
        var weekNumber = 2;
        var lessonDate = TestConstants.FirstDayOfTerm2024.AddDays((weekNumber - 1) * 7);
        await AddWeekPlannerToDatabase(_fixture, _teacher, lessonDate, weekNumber);
        await CreateLessonPlan(_fixture, _teacher, 3, lessonDate);
        await CreateLessonPlan(_fixture, _teacher, 4, lessonDate);
        
        var dbContext = _fixture.CreateDbContext();
        var lessonPlan = dbContext.LessonPlans.First(lp => lp.StartPeriod == 3);
        var lessonPlanRequest = new UpdateLessonPlanRequest(
            lessonPlan.Id.Value,
            _subject.Id.Value,
            [],
            "",
            "",
            [],
            lessonDate,
            2,
            3);

        var result = await _client.PatchAsJsonAsync($"/api/{_teacher.Id.Value}/lesson-plans", lessonPlanRequest);

        var lessonPlans = await dbContext.LessonPlans
            .Where(lp => lp.LessonDate == TestConstants.FirstDayOfTerm2024.AddDays((weekNumber - 1) * 7))
            .ToListAsync();

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        lessonPlans.FirstOrDefault(lp => lp.StartPeriod == 3).Should().NotBeNull();
        lessonPlans.FirstOrDefault(lp => lp.StartPeriod == 4).Should().BeNull();
    }
    
}