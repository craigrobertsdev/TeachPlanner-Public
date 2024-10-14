using System.Net;
using FluentAssertions;
using TeachPlanner.Api.IntegrationTests.Helpers;

namespace TeachPlanner.Api.IntegrationTests.Features.Account;

public class DeleteAccountTests : IClassFixture<MySqlFixture>
{
    private readonly MySqlFixture _fixture;
    private readonly HttpClient _client;

    public DeleteAccountTests(MySqlFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient();
    }

    [Fact]
    public async Task ShouldDeleteUserAndAllAssociatedData()
    {
        var user = await AccountHelpers.CreateUser(_fixture);
        await using var dbContext = _fixture.CreateDbContext();
        var teacher = await AccountHelpers.CreateTeacher(_fixture, user);

        var result = await _client.DeleteAsync($"/api/{teacher.Id.Value}/");
        
        result.Should().HaveStatusCode(HttpStatusCode.OK);
        dbContext.Teachers.Should().NotContain(teacher);
        dbContext.Users.Should().NotContain(user);
        dbContext.WeekStructures.Where(wp => wp.TeacherId == teacher.Id).Should().BeEmpty();
        dbContext.YearData.FirstOrDefault(yd => yd.TeacherId == teacher.Id).Should().BeNull();
    }
    
    [Fact]
    public async Task ShouldReturnNotFoundWhenTeacherDoesNotExist()
    {
        var result = await _client.DeleteAsync($"/api/{Guid.NewGuid()}/");
        
        result.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }
}
