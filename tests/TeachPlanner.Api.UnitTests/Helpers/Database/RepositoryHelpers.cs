using FakeItEasy;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.Database.Repositories;
using TeachPlanner.Api.Interfaces.Persistence;

namespace TeachPlanner.Api.Tests.Helpers.Database;

public static class RepositoryHelpers
{
    private static readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private static readonly IPublisher _publisher;

    static RepositoryHelpers()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TeachPlanner")
            .Options;

        _publisher = A.Fake<IPublisher>();
    }

    private static ApplicationDbContext CreateContext()
    {
        return new ApplicationDbContext(_dbContextOptions, _publisher);
    }

    public static ILessonPlanRepository CreateTestLessonPlanRepository()
    {
        return new LessonPlanRepository(CreateContext());
    }

    public static IWeekPlannerRepository CreateTestWeekPlannerRepository()
    {
        return new WeekPlannerRepository(CreateContext());
    }

    public static IYearDataRepository CreateTestYearDataRepository()
    {
        return new YearDataRepository(CreateContext());
    }

    public static ISubjectRepository CreateTestSubjectRepository()
    {
        return new SubjectRepository(CreateContext());
    }

    public static ICurriculumRepository CreateTestCurriculumRepository()
    {
        return new CurriculumRepository(CreateContext());
    }

    public static ITeacherRepository CreateTestTeacherRepository()
    {
        return new TeacherRepository(CreateContext());
    }
}