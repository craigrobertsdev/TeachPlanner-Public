using Microsoft.EntityFrameworkCore;
using TeachPlanner.Shared.Database;
using TeachPlanner.Shared.Domain.LessonPlans;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.YearDataRecords;
using FakeItEasy;
using MediatR;

namespace TeachPlanner.Api.UnitTests.Database;
public class LessonPlanRespositoryTests
{
    private async Task<ApplicationDbContext> GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TeachPlanner")
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
                "Planning Notes HTML",
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
