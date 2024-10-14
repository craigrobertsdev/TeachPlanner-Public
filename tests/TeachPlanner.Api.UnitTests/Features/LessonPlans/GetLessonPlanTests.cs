using FakeItEasy;
using FluentAssertions;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Features.LessonPlans;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Interfaces.Services;
using TeachPlanner.Api.Tests.Helpers.Domain;
using TeachPlanner.Shared.Contracts.LessonPlans;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Features.LessonPlans;

public class GetLessonPlanTests
{
    private readonly ICurriculumService _curriculumService;
    private readonly ILessonPlanRepository _lessonPlanRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IYearDataRepository _yearDataRepository;

    public GetLessonPlanTests()
    {
        _teacherRepository = A.Fake<ITeacherRepository>();
        _yearDataRepository = A.Fake<IYearDataRepository>();
        _lessonPlanRepository = A.Fake<ILessonPlanRepository>();
        _curriculumService = A.Fake<ICurriculumService>();
    }

    [Fact]
    public async Task Handle_WhenLessonPlanExists_ReturnLessonPlan()
    {
        // Arrange
        var teacher = TeacherHelpers.CreateTeacher();
        teacher.AddResource(ResourceHelpers.CreateBasicResource(teacher.Id));
        var yearLevels = TeacherHelpers.CreateYearLevelsTaught();
        var query = new GetLessonPlan.Query(teacher.Id, new DateOnly(2024, 1, 29), 1, false);
        var subjects = SubjectHelpers.CreateCurriculumSubjects();
        A.CallTo(() => _teacherRepository.GetById(teacher.Id, default)).Returns(teacher);
        A.CallTo(() =>
                _lessonPlanRepository.GetByYearDataAndDateAndPeriod(A<YearDataId>._, A<DateOnly>._, A<int>._, default))
            .Returns(LessonPlanHelpers.CreateLessonPlan(subjects[0].Id));
        A.CallTo(() => _curriculumService.GetSubjectsByYearLevels(A<IEnumerable<SubjectId>>._, yearLevels))
            .Returns(subjects);
        A.CallTo(() => _yearDataRepository.GetYearLevelsTaught(teacher.Id, 2024, default)).Returns(yearLevels);
        var handler = new GetLessonPlan.Handler(_teacherRepository, _yearDataRepository, _curriculumService,
            _lessonPlanRepository);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        AssertionOptions.FormattingOptions.MaxDepth = 100;
        result.Should().BeOfType<LessonPlanResponse>();
        result.Curriculum.Count().Should().Be(subjects.Count);
    }

    [Fact]
    public async Task Handle_WhenResourcesAreLinkedToLessonPlan_IncludesThem()
    {
        // Arrange
        var subjects = SubjectHelpers.CreateCurriculumSubjects();
        var teacher = TeacherHelpers.CreateTeacher();
        var resources = ResourceHelpers.CreateResources(teacher.Id, subjects.Select(s => s.Id).ToList());
        teacher.AddResources(resources);
        var yearLevels = TeacherHelpers.CreateYearLevelsTaught();
        var filteredResources = resources.Where(r => r.SubjectId == resources[0].SubjectId).ToList();
        var lessonPlan = LessonPlanHelpers.CreateLessonPlan(resources[0].SubjectId, filteredResources);
        A.CallTo(() => _teacherRepository.GetById(teacher.Id, default)).Returns(teacher);
        A.CallTo(() =>
                _lessonPlanRepository.GetByYearDataAndDateAndPeriod(A<YearDataId>._, A<DateOnly>._, A<int>._, default))
            .Returns(lessonPlan);
        A.CallTo(() => _lessonPlanRepository.GetResources(lessonPlan, default)).Returns(filteredResources);
        A.CallTo(() => _curriculumService.GetSubjectsByYearLevels(A<IEnumerable<SubjectId>>._, yearLevels))
            .Returns(subjects);
        A.CallTo(() => _yearDataRepository.GetYearLevelsTaught(teacher.Id, 2024, default)).Returns(yearLevels);
        var handler = new GetLessonPlan.Handler(_teacherRepository, _yearDataRepository, _curriculumService,
            _lessonPlanRepository);
        var query = new GetLessonPlan.Query(teacher.Id, new DateOnly(2024, 1, 29), 1, false);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.LessonPlan.Should().NotBeNull();
        result.LessonPlan!.Resources[0].Id.Should().Be(resources[0].Id);
        result.LessonPlan!.Resources.Count.Should().Be(filteredResources.Count);
    }

    [Fact]
    public async Task Handle_WhenTeacherDoesNotExist_ThrowTeacherNotFoundException()
    {
        // Arrange
        var teacher = TeacherHelpers.CreateTeacher();
        var yearLevels = TeacherHelpers.CreateYearLevelsTaught();
        var query = new GetLessonPlan.Query(teacher.Id, new DateOnly(2024, 1, 29), 1, false);
        A.CallTo(() => _teacherRepository.GetById(teacher.Id, default)).Returns((Teacher?)null);
        var handler = new GetLessonPlan.Handler(_teacherRepository, _yearDataRepository, _curriculumService,
            _lessonPlanRepository);

        // Act
        Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<TeacherNotFoundException>();
    }

    // [Fact]
    // public async Task Handle_WhenContentDescriptionsAreOnLessonPlan_TheyAreFilteredCorrectly()
    // {

    // }
}