using FakeItEasy;
using FluentAssertions;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Features.LessonPlans;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Tests.Helpers.Domain;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Features.LessonPlans;

public class CheckLessonOverlapTests
{
    private readonly ILessonPlanRepository _lessonPlanRepository;
    private readonly IYearDataRepository _yearDataRepository;

    public CheckLessonOverlapTests()
    {
        _lessonPlanRepository = A.Fake<ILessonPlanRepository>();
        _yearDataRepository = A.Fake<IYearDataRepository>();
    }

    [Fact]
    public async Task WhenNoLessonPlans_ReturnsFalse()
    {
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._))
            .Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._))
            .Returns([]);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._))
            .Returns([]);

        // Act
        var result = await CheckLessonOverlap.Endpoint(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2024, 1, 29),
            1,
            1,
            _yearDataRepository,
            _lessonPlanRepository,
            CancellationToken.None);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public async Task WhenNoOverlaps_ReturnsFalse()
    {
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan>
        {
            LessonPlanHelpers.CreateLessonPlan(1, 1), LessonPlanHelpers.CreateLessonPlan(3, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._))
            .Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._))
            .Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Endpoint(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2024, 1, 29), 2, 1,
            _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(false);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(4, 2)]
    public async Task WhenOverlapExistsAndLessonPlanIdIsNotAlreadyInTheDatabase_ReturnTrue(int startPeriod,
        int numberOfLessons)
    {
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan>
        {
            LessonPlanHelpers.CreateLessonPlan(1, 2),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._))
            .Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._))
            .Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Endpoint(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2024, 1, 29),
            startPeriod, numberOfLessons, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public async Task WhenLessonPlanExistsAndNumberOfPeriodsHasNotChanged_ReturnsFalse()
    {
        // If the lessonPlan.StartPeriod is the same as what the database already records && the numberOfLessons is the same, return false  
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan>
        {
            LessonPlanHelpers.CreateLessonPlan(1, 2),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._))
            .Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._))
            .Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Endpoint(Guid.NewGuid(), lessonPlans[0].Id.Value,
            new DateOnly(2024, 1, 29), 1, 2, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public async Task WhenLessonPlanExistsAndNumberOfPeriodsChangesCausingOverlap_ReturnsTrue()
    {
        // If the lessonPlan.StartPeriod is the same as what the database already records && the numberOfLessons is different, causing an overlap, return true
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan>
        {
            LessonPlanHelpers.CreateLessonPlan(1, 1),
            LessonPlanHelpers.CreateLessonPlan(2, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._))
            .Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._))
            .Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Endpoint(Guid.NewGuid(), lessonPlans[0].Id.Value,
            new DateOnly(2024, 1, 29), 1, 2, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public async Task WhenLessonPlanExistsAndNumberOfPeriodsChangesCausingNoOverlap_ReturnsFalse()
    {
        // If the lessonPlan.StartPeriod is the same as what the database already records && the numberOfLessons is different, causing no overlap, return false 
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan>
        {
            LessonPlanHelpers.CreateLessonPlan(1, 1),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._))
            .Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._))
            .Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Endpoint(Guid.NewGuid(), lessonPlans[0].Id.Value,
            new DateOnly(2024, 1, 29), 1, 2, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public async Task WhenLessonPlanExistsAndStartPeriodChangesCausingOverlap_ReturnsTrue()
    {
        // If the lessonPlan.StartPeriod is different from what the database already records && an overlap is caused, return true
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan>
        {
            LessonPlanHelpers.CreateLessonPlan(1, 1),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._))
            .Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._))
            .Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Endpoint(Guid.NewGuid(), lessonPlans[0].Id.Value,
            new DateOnly(2024, 1, 29), 3, 2, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public async Task WhenLessonPlanExistsAndStartPeriodChangesCausingNoOverlap_ReturnsFalse()
    {
        // If the lessonPlan.StartPeriod is different from what the database already records && no overlap is caused, return false
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan>
        {
            LessonPlanHelpers.CreateLessonPlan(1, 1),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._))
            .Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._))
            .Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Endpoint(Guid.NewGuid(), lessonPlans[0].Id.Value,
            new DateOnly(2024, 1, 29), 2, 2, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(true);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    public async Task WhenLessonStartOrNumberOfPeriodsOutOfBounds_ShouldError(int startPeriod, int numberOfLessons)
    {
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan>
        {
            LessonPlanHelpers.CreateLessonPlan(1, 2),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._))
            .Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._))
            .Returns(lessonPlans);

        // Act
        var act = async () => await CheckLessonOverlap.Endpoint(Guid.NewGuid(), Guid.NewGuid(),
            new DateOnly(2024, 1, 29), startPeriod, numberOfLessons, _yearDataRepository, _lessonPlanRepository,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }
}