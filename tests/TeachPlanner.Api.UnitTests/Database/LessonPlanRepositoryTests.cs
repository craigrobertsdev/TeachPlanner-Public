using FakeItEasy;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Database;

public class LessonPlanRepositoryTests
{
    private async Task<ApplicationDbContext> GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TeachPlanner")
            .Options;

        var publisher = A.Fake<IPublisher>();
        var databaseContext = new ApplicationDbContext(options, publisher);
        databaseContext.Database.EnsureDeleted();
        databaseContext.Database.EnsureCreated();

        if (!await databaseContext.TermPlanners.AnyAsync())
        {
            var lessonPlan = LessonPlan.Create(
                new YearDataId(Guid.NewGuid()),
                new SubjectId(Guid.NewGuid()),
                [],
                "Planning Notes",
                "Planning Notes Html",
                1,
                1,
                new DateOnly(2023, 10, 9),
                []);

            databaseContext.LessonPlans.Add(lessonPlan);
        }

        // if there are any change tracking issues, uncomment this
        //databaseContext.TermPlanners.AsNoTracking();

        await databaseContext.SaveChangesAsync();

        return databaseContext;
    }
}