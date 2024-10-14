using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.IntegrationTests.Helpers;

namespace TeachPlanner.Api.IntegrationTests.Features.Teachers;

public class AccountSetupTests : IClassFixture<MySqlFixture>
{
    private readonly HttpClient _client;
    private readonly MySqlFixture _fixture;
    private readonly Teacher _teacher;

    public AccountSetupTests(MySqlFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
        _teacher = fixture.CreateDbContext().Teachers.First();
    }

    [Fact]
    public async Task WhenPassedInvalidPeriodTime_DelegateShouldThrowException()
    {
        // Arrange
        var accountSetupRequest = TeacherHelpers.CreateAccountSetupRequestWithOverlappingTimes();

        // Act
        var response = await _client.PostAsJsonAsync($"api/{_teacher.Id.Value}/setup", accountSetupRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task WhenPassedValidData_ShouldSetUpAccount()
    {
        var accountSetupRequest = TeacherHelpers.CreateAccountSetupRequest();

        var response =
            await _client.PostAsJsonAsync($"api/{_teacher.Id.Value}/setup?calendarYear=2024", accountSetupRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        await using var dbContext = _fixture.CreateDbContext();
        var teacher = dbContext.Teachers
            .First();
        teacher.AccountSetupComplete.Should().Be(true);
        teacher.LastSelectedYear.Should().Be(2024);
        teacher.SubjectsTaught.Count.Should().Be(accountSetupRequest.SubjectsTaught.Count);
    }

    [Fact]
    public async Task WhenPassedInvalidPeriodTime_HandlerShouldThrowException()
    {
        var accountSetupRequest = TeacherHelpers.CreateAccountSetupRequest();
        accountSetupRequest.WeekStructure.Periods[0].EndTime =
            accountSetupRequest.WeekStructure.Periods[0].EndTime.AddHours(-1);
        var dbContext = _fixture.CreateDbContext();
        var teacher = dbContext.Teachers.First();

        var response =
            await _client.PostAsJsonAsync($"api/{teacher.Id.Value}/setup?calendarYear=2024", accountSetupRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenPassedOverlappingPeriodTimes_ShouldThrowException()
    {
        var accountSetupRequest = TeacherHelpers.CreateAccountSetupRequest();
        accountSetupRequest.WeekStructure.Periods[0].EndTime =
            accountSetupRequest.WeekStructure.Periods[0].EndTime.AddMinutes(30);
        var dbContext = _fixture.CreateDbContext();
        var teacher = dbContext.Teachers.First();

        var response =
            await _client.PostAsJsonAsync($"api/{teacher.Id.Value}/setup?calendarYear=2024", accountSetupRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
    }
}