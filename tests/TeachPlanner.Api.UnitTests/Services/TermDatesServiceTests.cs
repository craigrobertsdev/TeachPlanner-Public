using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeachPlanner.Api.Services;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Services;
using TeachPlanner.Shared.Database;
using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.Api.UnitTests.Services;

public class TermDatesServiceTests
{
    private readonly ITermDatesService _termDatesService;
    private readonly List<TermDate> _termDates;
    private readonly IServiceProvider _serviceProvider;

    public TermDatesServiceTests()
    {
        _termDates = new List<TermDate> {
            new(1, new DateOnly(2023, 1, 30), new DateOnly(2023, 4, 14)),
            new(2, new DateOnly(2023, 5, 1), new DateOnly(2023, 7, 7)),
            new(3, new DateOnly(2023, 7, 24), new DateOnly(2023, 9, 29)),
            new(4, new DateOnly(2023, 10, 16), new DateOnly(2023, 12, 15))
        };

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TeachPlanner")
            .Options;

        var publisher = A.Fake<IPublisher>();
        var context = new ApplicationDbContext(options, publisher);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        context.TermDates.AddRange(_termDates);
        context.SaveChanges();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(provider => context);
        _serviceProvider = serviceCollection.BuildServiceProvider();
        _termDatesService = new TermDatesService(_serviceProvider);
    }

    [Fact]
    public void GetWeekStart_WhenPassedValidArguments_ReturnsCorrectDate()
    {
        // Arrange
        var termNumber = 2;
        var weekNumber = 5;

        // Act
        var result = _termDatesService.GetWeekStart(2023, termNumber, weekNumber);

        // Assert
        result.Should().Be(new DateOnly(2023, 5, 29));
    }

    [Fact]
    public void WeekStart_WhenPassedInvalidTermNumber_ThrowsArgumentException()
    {
        // Arrange
        var termNumber = 5;
        var weekNumber = 5;

        // Act
        Action act = () => _termDatesService.GetWeekStart(2023, termNumber, weekNumber);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WeekStart_WhenPassedInvalidWeekNumber_ThrowsArgumentException()
    {
        // Arrange
        var termNumber = 2;
        var weekNumber = -1;

        // Act
        Action act = () => _termDatesService.GetWeekStart(2023, termNumber, weekNumber);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetWeekNumber_WhenPassedValidArguments_ReturnsCorrectWeekNumber()
    {
        // Arrange
        var termNumber = 2;
        var weekStart = new DateOnly(2023, 5, 29);

        // Act
        var result = _termDatesService.GetWeekNumber(2023, termNumber, weekStart);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void GetWeekNumber_WhenPassedMidweekValue_ReturnsCorrectWeekNumber()
    {
        // Arrange
        var termNumber = 2;
        var weekStart = new DateOnly(2023, 5, 31);

        // Act
        var result = _termDatesService.GetWeekNumber(2023, termNumber, weekStart);

        // Assert
        result.Should().Be(5);
    }


    [Fact]
    public void GetWeekNumber_WhenPassedOutOfRangeTermNumber_ThrowsException()
    {
        // Arrange
        var termNumber = 5;
        var weekStart = new DateOnly(2023, 5, 31);

        // Act
        var act = () => _termDatesService.GetWeekNumber(2023, termNumber, weekStart);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TermWeeks_WhenCalled_ReturnsCorrectNumberOfTermWeeks()
    {
        // Arrange
        var expected = new Dictionary<int, Dictionary<int, int>>() {
            { 2023, new Dictionary<int, int>() {
                { 1, 11 },
                { 2, 10 },
                { 3, 10 },
                { 4, 9 }
            } }
        };

        // Act
        var result = _termDatesService.TermWeekNumbers;

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetTermNumber_WhenPassedValidDate_ReturnsCorrectTermNumber()
    {
        // Arrange
        var date = new DateOnly(2023, 5, 31);

        // Act
        var result = _termDatesService.GetTermNumber(date);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void GetTermNumber_WhenPassedInvalidYear_ShouldThrowException()
    {
        // Arrange
        var date = new DateOnly(2021, 5, 31);

        // Act
        var act = () => _termDatesService.GetTermNumber(date);

        // Assert
        act.Should().Throw<TermDatesNotFoundException>();
    }
}
