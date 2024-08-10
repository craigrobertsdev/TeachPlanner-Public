using FakeItEasy;
using FluentAssertions;
using TeachPlanner.Api.Features.LessonPlans;
using TeachPlanner.Api.UnitTests.Helpers;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Domain.LessonPlans;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.YearDataRecords;

namespace TeachPlanner.Api.UnitTests.Features.LessonPlans;
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
    public async void WhenNoLessonPlans_ReturnsFalse()
    {
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._)).Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._)).Returns(new List<LessonPlan>());
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._)).Returns([]);

        // Act
        var result = await CheckLessonOverlap.Delegate(
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
    public async void WhenNoOverlaps_ReturnsFalse()
    {
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan> {
            LessonPlanHelpers.CreateLessonPlan(1, 1),
            LessonPlanHelpers.CreateLessonPlan(3, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._)).Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._)).Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Delegate(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2024, 1, 29), 2, 1, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(false);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(4, 2)]
    public async void WhenOverlapExistsAndLessonPlanIdIsNotAlreadyInTheDatabase_ReturnTrue(int startPeriod, int numberOfLessons)
    {
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan> {
            LessonPlanHelpers.CreateLessonPlan(1, 2),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._)).Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._)).Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Delegate(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2024, 1, 29), startPeriod, numberOfLessons, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public async void WhenLessonPlanExistsAndNumberOfPeriodsHasNotChanged_ReturnsFalse()
    {
        // If the lessonPlan.StartPeriod is the same as what the database already records && the numberOfLessons is the same, return false  
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan> {
            LessonPlanHelpers.CreateLessonPlan(1, 2),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._)).Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._)).Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Delegate(Guid.NewGuid(), lessonPlans[0].Id.Value, new DateOnly(2024, 1, 29), 1, 2, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public async void WhenLessonPlanExistsAndNumberOfPeriodsChangesCausingOverlap_ReturnsTrue()
    {
        // If the lessonPlan.StartPeriod is the same as what the database already records && the numberOfLessons is different, causing an overlap, return true
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan> {
            LessonPlanHelpers.CreateLessonPlan(1, 1),
            LessonPlanHelpers.CreateLessonPlan(2, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._)).Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._)).Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Delegate(Guid.NewGuid(), lessonPlans[0].Id.Value, new DateOnly(2024, 1, 29), 1, 2, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public async void WhenLessonPlanExistsAndNumberOfPeriodsChangesCausingNoOverlap_ReturnsFalse()
    {
        // If the lessonPlan.StartPeriod is the same as what the database already records && the numberOfLessons is different, causing no overlap, return false 
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan> {
            LessonPlanHelpers.CreateLessonPlan(1, 1),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._)).Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._)).Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Delegate(Guid.NewGuid(), lessonPlans[0].Id.Value, new DateOnly(2024, 1, 29), 1, 2, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(false);
    }

    [Fact]
    public async void WhenLessonPlanExistsAndStartPeriodChangesCausingOverlap_ReturnsTrue()
    {
        // If the lessonPlan.StartPeriod is different from what the database already records && an overlap is caused, return true
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan> {
            LessonPlanHelpers.CreateLessonPlan(1, 1),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._)).Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._)).Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Delegate(Guid.NewGuid(), lessonPlans[0].Id.Value, new DateOnly(2024, 1, 29), 3, 2, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public async void WhenLessonPlanExistsAndStartPeriodChangesCausingNoOverlap_ReturnsFalse()
    {
        // If the lessonPlan.StartPeriod is different from what the database already records && no overlap is caused, return false
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan> {
            LessonPlanHelpers.CreateLessonPlan(1, 1),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._)).Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._)).Returns(lessonPlans);

        // Act
        var result = await CheckLessonOverlap.Delegate(Guid.NewGuid(), lessonPlans[0].Id.Value, new DateOnly(2024, 1, 29), 2, 2, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        result.Should().Be(true);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(0, 1)]
    public async void WhenLessonStartOrNumberOfPeriodsOutOfBounds_ShouldError(int startPeriod, int numberOfLessons)
    {
        // Arrange
        var yearData = YearDataHelpers.CreateYearData();
        var lessonPlans = new List<LessonPlan> {
            LessonPlanHelpers.CreateLessonPlan(1, 2),
            LessonPlanHelpers.CreateLessonPlan(3, 1),
            LessonPlanHelpers.CreateLessonPlan(5, 1)
        };

        A.CallTo(() => _yearDataRepository.GetByTeacherIdAndYear(A<TeacherId>._, A<int>._, A<CancellationToken>._)).Returns(yearData);
        A.CallTo(() => _lessonPlanRepository.GetByDate(A<YearDataId>._, A<DateOnly>._, A<CancellationToken>._)).Returns(lessonPlans);

        // Act
        var act = async () => await CheckLessonOverlap.Delegate(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2024, 1, 29), startPeriod, numberOfLessons, _yearDataRepository, _lessonPlanRepository, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }
}
