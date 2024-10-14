using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.IntegrationTests.Helpers;
using TeachPlanner.Shared.Contracts.LessonPlans.CreateLessonPlan;
using static TeachPlanner.Api.IntegrationTests.Helpers.WeekPlannerHelpers;

namespace TeachPlanner.Api.IntegrationTests.Features.LessonPlans;

public class CreateLessonPlanTests : IClassFixture<MySqlFixture>
{
    private readonly MySqlFixture _fixture;
    private readonly Teacher _teacher;
    private readonly HttpClient _client;
    private readonly CurriculumSubject _subject;

    public CreateLessonPlanTests(MySqlFixture fixture)
    {
        _fixture = fixture;
        using var dbContext = fixture.CreateDbContext();
        _teacher = dbContext.Teachers.First();
        _client = _fixture.CreateClient();
        _subject = dbContext.CurriculumSubjects.First();
    }

    [Fact]
    public async Task WhenNoOverlap_ShouldCreateNewLessonPlan()
    {
        // Arrange
        await AddWeekPlannerToDatabase(_fixture, _teacher, TestConstants.FirstDayOfTerm2024, 1);
        var lessonPlanRequest = new CreateLessonPlanRequest(
            _subject.Id.Value,
            [],
            "",
            "",
            [],
            TestConstants.FirstDayOfTerm2024,
            1,
            5);

        // Act 
        var result = await _client.PostAsJsonAsync($"/api/{_teacher.Id.Value}/lesson-plans", lessonPlanRequest);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var dbContext = _fixture.CreateDbContext();
        var lessonPlans = await dbContext.LessonPlans.ToListAsync();
        lessonPlans.Should().HaveCount(2);
        var lessonPlan = lessonPlans.First(lp => lp.StartPeriod == 5);
        lessonPlan.Should().NotBeNull();
    }

    [Fact]
    public async Task WhenOverlapExists_ShouldDeleteOldLessonPlanAndUpdate()
    {
        // Arrange
        var weekNumber = 3;
        var startPeriodForOverwrittenLesson = 3;
        var lessonDate = TestConstants.FirstDayOfTerm2024.AddDays((weekNumber - 1) * 7);
        await AddWeekPlannerToDatabase(_fixture, _teacher, lessonDate, weekNumber);
        await CreateLessonPlan(_fixture, _teacher, startPeriodForOverwrittenLesson, lessonDate);
        var dbContext = _fixture.CreateDbContext();
        var lessonPlanRequest = new CreateLessonPlanRequest(
            _subject.Id.Value,
            [],
            "",
            "",
            [],
            lessonDate,
            2,
            2);

        // Act
        await _client.PostAsJsonAsync($"/api/{_teacher.Id.Value}/lesson-plans", lessonPlanRequest);

        // Assert
        var lessonPlans = await dbContext.LessonPlans
            .Where(lp => lp.LessonDate == lessonDate)
            .ToListAsync();
        lessonPlans.Should().HaveCount(2);
        var deletedLessonPlan = lessonPlans.FirstOrDefault(lp => lp.StartPeriod == startPeriodForOverwrittenLesson);
        deletedLessonPlan.Should().BeNull();
        var lessonPlan = lessonPlans.First(lp => lp.StartPeriod == 2);
        lessonPlan.NumberOfLessons.Should().Be(2);
    }

    [Fact]
    public async Task WhenNoLessonPlanExistsButOverlapDoes_DeletesOverlappedLessonPlansAndCreatesNewOne()
    {
        var weekNumber = 4;
        var lessonDate = TestConstants.FirstDayOfTerm2024.AddDays((weekNumber - 1) * 7);
        await AddWeekPlannerToDatabase(_fixture, _teacher, lessonDate, weekNumber);
        await CreateLessonPlan(_fixture, _teacher, 4, lessonDate);
        var lessonPlanRequest = new CreateLessonPlanRequest(
            _subject.Id.Value,
            [],
            "",
            "",
            [],
            lessonDate,
            2,
            3);

        await _client.PostAsJsonAsync($"/api/{_teacher.Id.Value}/lesson-plans", lessonPlanRequest);

        var dbContext = _fixture.CreateDbContext();
        var lessonPlans = await dbContext.LessonPlans
            .Where(lp => lp.LessonDate == TestConstants.FirstDayOfTerm2024.AddDays((weekNumber - 1) * 7))
            .ToListAsync();

        lessonPlans.FirstOrDefault(lp => lp.StartPeriod == 4).Should().BeNull();
        lessonPlans.First(lp => lp.StartPeriod == 3).Should().NotBeNull();
    }


}