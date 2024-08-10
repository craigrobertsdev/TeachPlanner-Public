using FakeItEasy;
using FluentAssertions;
using MediatR;
using TeachPlanner.Api.Features.Teachers;
using TeachPlanner.Api.UnitTests.Helpers;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.Teachers.AccountSetup;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Api.UnitTests.Features.Teachers;
public class AccountSetupTests
{
    private readonly DayPlanPatternDto _dayPlanPatternDto;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IYearDataRepository _yearDataRepository;
    private readonly ICurriculumRepository _curriculumRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISender _sender;
    private readonly AccountSetup.Validator _validator;

    public AccountSetupTests()
    {
        _teacherRepository = A.Fake<ITeacherRepository>();
        _yearDataRepository = A.Fake<IYearDataRepository>();
        _curriculumRepository = A.Fake<ICurriculumRepository>();
        _unitOfWork = A.Fake<IUnitOfWork>();
        _dayPlanPatternDto = TeacherHelpers.CreateDayPlanPatternDto();
        _sender = A.Fake<ISender>();
        _validator = new();
    }

    [Fact]
    public void Handle_WhenPassedInvalidPeriodTime_ShouldThrowException()
    {
        // Arrange
        var subjectsTaught = TeacherHelpers.CreateSubjectNames();
        var yearLevelsTaught = TeacherHelpers.CreateYearLevelsTaught();
        var termDates = TeacherHelpers.CreateTermDates();
        var weekStructure = TeacherHelpers.CreateDayPlanTemplate(_dayPlanPatternDto);
        weekStructure.Periods[0].StartTime.AddHours(-9);
        var command = new AccountSetup.Command(subjectsTaught, yearLevelsTaught, weekStructure, new TeacherId(Guid.NewGuid()), 2023);
        var handler = new AccountSetup.Handler(_teacherRepository, _curriculumRepository, _yearDataRepository, _unitOfWork);

        // Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        // Assert
        act.Should().ThrowAsync<CreateTimeFromDtoException>();
    }

    [Fact]
    public void Handle_WhenPassedOverlappingPeriodTimes_ShouldThrowException()
    {
        // Arrange
        var subjectsTaught = TeacherHelpers.CreateSubjectNames();
        var yearLevelsTaught = TeacherHelpers.CreateYearLevelsTaught();
        var termDates = TeacherHelpers.CreateTermDates();
        var weekStructure = TeacherHelpers.CreateDayPlanTemplate(_dayPlanPatternDto);
        weekStructure.Periods[0].StartTime.AddHours(1);
        var command = new AccountSetup.Command(subjectsTaught, yearLevelsTaught, weekStructure, new TeacherId(Guid.NewGuid()), 2023);
        var handler = new AccountSetup.Handler(_teacherRepository, _curriculumRepository, _yearDataRepository, _unitOfWork);

        // Act
        Func<Task> act = () => handler.Handle(command, new CancellationToken());

        // Assert
        act.Should().ThrowAsync<CreateTimeFromDtoException>();
    }

    [Fact]
    public void Delegate_WhenPassedInvalidPeriodTime_ShouldThrowException()
    {
        // Arrange
        var accountSetupRequest = TeacherHelpers.CreateAccountSetupRequestWithOverlappingTimes();

        // Act
        Func<Task> act = () => AccountSetup.Delegate(Guid.NewGuid(), accountSetupRequest, 2023, _sender, _validator, A<CancellationToken>._);

        // Assert
        act.Should().ThrowAsync<CreateTimeFromDtoException>();
    }

    [Fact]
    public void Delegate_WhenPassedOverlappingTermDate_ShouldThrowException()
    {
        // Arrange
        var accountSetupRequest = TeacherHelpers.CreateAccountSetupRequestWithOverlappingDates();

        // Act
        Func<Task> act = () => AccountSetup.Delegate(Guid.NewGuid(), accountSetupRequest, 2023, _sender, _validator, A<CancellationToken>._);

        // Assert
        act.Should().ThrowAsync<CreateTimeFromDtoException>();
    }

    [Fact]
    public async void Delegate_WhenPassedValidData_ShouldCreateDayPlanTemplate()
    {
        // Arrange
        var accountSetupRequest = TeacherHelpers.CreateAccountSetupRequest();
        var sender = A.Fake<ISender>();

        // Act
        await AccountSetup.Delegate(Guid.NewGuid(), accountSetupRequest, 2023, sender, _validator, CancellationToken.None);

        // Assert
        var call = Fake.GetCalls(sender).First();
        AccountSetup.Command command = (AccountSetup.Command)call.Arguments[0]!;
        command.WeekStructure.Periods.Should().Equal(TeacherHelpers.CreateDayPlanTemplate(_dayPlanPatternDto).Periods);
    }

    [Fact]
    public async void Handler_WhenPassedValidData_ShouldUpdateTeacherSubjectsTaught()
    {
        // Arrange
        var subjects = TeacherHelpers.CreateSubjectNames();
        var weekStructure = TeacherHelpers.CreateDayPlanTemplate(_dayPlanPatternDto);
        var yearLevelsTaught = TeacherHelpers.CreateYearLevelsTaught();
        var termDates = TeacherHelpers.CreateTermDates();
        var teacher = TeacherHelpers.CreateTeacher();
        var subjectsTaught = TeacherHelpers.CreateCurriculumSubjects(subjects);

        A.CallTo(() => _teacherRepository.GetById(teacher.Id, A<CancellationToken>._))
            .Returns(teacher);

        A.CallTo(() => _curriculumRepository.GetSubjectsByName(subjects, A<CancellationToken>._))
            .Returns(subjectsTaught);

        var command = new AccountSetup.Command(subjects, yearLevelsTaught, weekStructure, teacher.Id, 2023);
        var handler = new AccountSetup.Handler(_teacherRepository, _curriculumRepository, _yearDataRepository, _unitOfWork);

        // Act
        await handler.Handle(command, new CancellationToken());

        // Assert
        teacher.SubjectsTaught.Should().Equal(subjectsTaught);
    }
}
