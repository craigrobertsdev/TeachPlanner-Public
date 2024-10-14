using FakeItEasy;
using FluentAssertions;
using TeachPlanner.Api.Features.Subjects;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Tests.Helpers.Domain;

namespace TeachPlanner.Api.Tests.Features.Subjects;

public class GetSubjectsTests
{
    private readonly ISubjectRepository _subjectRepository;

    public GetSubjectsTests()
    {
        _subjectRepository = A.Fake<ISubjectRepository>();
    }

    [Fact]
    public async Task Handler_WhenNoElaborationsRequested_ShouldReturnCurriculumSubjectsWithoutElaborations()
    {
        // Arrange
        var subjects = SubjectHelpers.CreateCurriculumSubjects();
        var query = new GetCurriculumSubjects.Query();
        var handler = new GetCurriculumSubjects.Handler(_subjectRepository);

        A.CallTo(() => _subjectRepository.GetCurriculumSubjects(A<CancellationToken>._)).Returns(subjects);

        // Act
        var result = await handler.Handle(query, new CancellationToken());

        // Assert
        result.Should().BeEquivalentTo(subjects);
    }
}