namespace TeachPlanner.Api.IntegrationTests.Features.YearData;

public class SetSubjectsTaughtTests(MySqlFixture fixture) : IClassFixture<MySqlFixture>
{

    [Fact]
    public async Task WhenPassedListOfSubjects_ShouldSetSubjectsTaughtByTeacher()
    {
        // TODO: Work out whether or not this endpoint is even needed.
        throw new NotImplementedException();
    }

    [Fact]
    public async Task ShouldRejectWhenNoSubjectsPassed()
    {
        throw new NotImplementedException();
    }
}