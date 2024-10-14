using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.Services;
using TeachPlanner.Api.Tests.Helpers.Domain;
using TeachPlanner.Shared.Enums;

namespace TeachPlanner.Api.Tests.Services;

public class CurriculumServiceTests
{
    private readonly ServiceProvider _serviceProvider;

    public CurriculumServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TeachPlanner")
            .Options;

        var publisher = A.Fake<IPublisher>();
        var context = new ApplicationDbContext(options, publisher);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        context.CurriculumSubjects.AddRange(SubjectHelpers.CreateCurriculumSubjects());
        context.SaveChanges();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(provider => context);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public void GetContentDescriptions_ReturnsExpected()
    {
        // Arrange
        var curriculumService = new CurriculumService(_serviceProvider);
        var subjectId = curriculumService.CurriculumSubjects[0].Id;
        var yearLevels = new List<YearLevelValue> { YearLevelValue.Reception, YearLevelValue.Year1 };

        // Act
        var result = curriculumService.GetContentDescriptions(subjectId, yearLevels);

        // Assert
        result.Keys.Count.Should().Be(yearLevels.Count);
        result.Keys.First().Should().Be(YearLevelValue.Reception);
        result[0].Count.Should().Be(curriculumService.CurriculumSubjects[0]
            .YearLevels.First(yl => yl.YearLevelValue == yearLevels[0]).GetContentDescriptions().Count);
    }

    [Fact]
    public void GetContentDescriptions_WhenSubjectHasBandLevels_ShouldReturnCorrectly()
    {
        // Arrange
        var curriculumService = new CurriculumService(_serviceProvider);
        var subjectId = curriculumService.CurriculumSubjects[1].Id;
        var yearLevels = new List<YearLevelValue> { YearLevelValue.Year2, YearLevelValue.Year3 };

        // Act
        var result = curriculumService.GetContentDescriptions(subjectId, yearLevels);

        // Assert
        result.Keys.Count.Should().Be(yearLevels.Count);
        result.Keys.First().Should().Be(YearLevelValue.Year2);
        var yearLevel =
            result.First().Value.Count.Should().Be(curriculumService.CurriculumSubjects[1].YearLevels
                .First(yl => yl.GetYearLevels().Contains(yearLevels[0])).GetContentDescriptions().Count);
    }
}