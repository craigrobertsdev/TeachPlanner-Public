using FakeItEasy;
using FluentAssertions;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.LessonPlans.CreateLessonPlan;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Api.Features.LessonPlans;
using TeachPlanner.Shared.Common.Interfaces.Services;

namespace TeachPlanner.Api.UnitTests.Features.LessonPlans;
public class CreateLessonPlanTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILessonPlanRepository _lessonPlanRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IWeekPlannerRepository _weekPlannerRepository;
    private readonly ITermDatesService _termDatesService;

    public CreateLessonPlanTests()
    {
        _unitOfWork = A.Fake<IUnitOfWork>();
        _lessonPlanRepository = A.Fake<ILessonPlanRepository>();
        _teacherRepository = A.Fake<ITeacherRepository>();
        _weekPlannerRepository = A.Fake<IWeekPlannerRepository>();
        _termDatesService = A.Fake<ITermDatesService>();
    }
}
