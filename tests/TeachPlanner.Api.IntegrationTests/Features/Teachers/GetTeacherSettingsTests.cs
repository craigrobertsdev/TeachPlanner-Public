using System.Net.Http.Json;
using FluentAssertions;
using TeachPlanner.Api.IntegrationTests.Helpers;
using TeachPlanner.Shared.Contracts.Teachers;

namespace TeachPlanner.Api.IntegrationTests.Features.Teachers;

public class GetTeacherSettingsTests : IClassFixture<MySqlFixture>
{
    private readonly MySqlFixture _fixture;
    private readonly HttpClient _client;

    public GetTeacherSettingsTests(MySqlFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient();
    }

    [Fact]
    public async Task WhenSettingsExist_ReturnsSettingsForRequestedYear()
    {
        var dbContext = _fixture.CreateDbContext();
        var teacher = dbContext.Teachers.First();

        var response = await _client.GetFromJsonAsync<SettingsResponse>($"/api/{teacher.Id.Value}/settings?calendarYear={teacher.LastSelectedYear}");

        response.Should().NotBeNull();
        response!.LastSelectedYear.Should().Be(teacher.LastSelectedYear);
    }

    [Fact]
    public async Task ReturnsLastSelectedYear()
    {
        var dbContext = _fixture.CreateDbContext();
        var teacher = dbContext.Teachers.First();
        teacher.SetLastSelectedYear(2025);
        dbContext.Teachers.Update(teacher);
        await dbContext.SaveChangesAsync();

        var response = await _client.GetFromJsonAsync<SettingsResponse>($"/api/{teacher.Id.Value}/settings?calendarYear={teacher.LastSelectedYear}");

        response.Should().NotBeNull();
        response!.LastSelectedYear.Should().Be(2025);
    }

    [Fact]
    public async Task ReturnsLastSelectedWeekStart()
    {
        var dbContext = _fixture.CreateDbContext();
        var teacher = dbContext.Teachers.First();

        var response = await _client.GetFromJsonAsync<SettingsResponse>($"/api/{teacher.Id.Value}/settings");

        response.Should().NotBeNull();
        response!.LastSelectedWeekStart.Should().Be(TestConstants.FirstDayOfTerm2024);
    }
}