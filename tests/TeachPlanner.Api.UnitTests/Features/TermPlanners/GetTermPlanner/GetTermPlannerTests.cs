using FakeItEasy;
using FluentAssertions;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.TermPlanners;
using TeachPlanner.Api.Domain.YearDataRecords;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Tests.Helpers.Domain;
using TeachPlanner.Shared.Contracts.TermPlanners;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Features.TermPlanners.GetTermPlanner;

public class GetTermPlannerTests
{
    private readonly ITeacherRepository _teacherRepository = A.Fake<ITeacherRepository>();
    private readonly ITermPlannerRepository _termPlannerRepository = A.Fake<ITermPlannerRepository>();

    [Fact]
    public async Task Handle_WhenCalledWithValidData_GetShouldReturnTermPlannerResult()
    {
        // Arrange
        var query = new Api.Features.TermPlanners.GetTermPlanner.Query(new TeacherId(Guid.NewGuid()), 2023);
        var teacher = Teacher.Create(Guid.NewGuid().ToString(), "Test", "Teacher");
        var yearData = YearData.Create(teacher.Id, 2023, DayPlanTemplateHelpers.CreateDayPlanTemplate(teacher.Id));
        teacher.AddYearData(YearDataEntry.Create(2023, yearData.Id));
        var termPlanner = TermPlanner.Create(new YearDataId(Guid.NewGuid()), 2023, []);
        var subjects = SubjectHelpers.CreateCurriculumSubjects();

        var handler = new Api.Features.TermPlanners.GetTermPlanner.Handler(_termPlannerRepository, _teacherRepository);

        A.CallTo(() => _teacherRepository.GetById(query.TeacherId, default)).Returns(teacher);
        A.CallTo(() => _termPlannerRepository.GetByYearDataIdAndYear(yearData.Id, 2023, A<CancellationToken>._))
            .Returns(termPlanner);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<TermPlannerDto>();
        // result.TermPlanner.Should().Be(termPlanner);
    }

    [Fact]
    public void Handle_WhenTeacherNotFound_ShouldThrowException()
    {
        // Arrange
        var query = new Api.Features.TermPlanners.GetTermPlanner.Query(new TeacherId(Guid.NewGuid()), 2023);
        A.CallTo(() => _teacherRepository.GetById(query.TeacherId, default)).Returns((Teacher)null!);
        var handler = new Api.Features.TermPlanners.GetTermPlanner.Handler(_termPlannerRepository, _teacherRepository);

        // Act
        Func<Task> act = () => handler.Handle(query, CancellationToken.None);

        // Assert
        act.Should().ThrowAsync<TeacherNotFoundException>();
    }
}