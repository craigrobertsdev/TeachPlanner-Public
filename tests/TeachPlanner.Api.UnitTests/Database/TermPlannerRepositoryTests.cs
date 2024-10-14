using FakeItEasy;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.TermPlanners;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Database;

public class TermPlannerRepositoryTests
{
    private readonly IPublisher _publisher = A.Fake<IPublisher>();

    private async Task<ApplicationDbContext> GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TeachPlanner")
            .Options;

        var databaseContext = new ApplicationDbContext(options, _publisher);
        databaseContext.Database.EnsureCreated();

        if (await databaseContext.TermPlanners.AnyAsync())
        {
            var termPlanner = TermPlanner.Create(new YearDataId(Guid.NewGuid()), 2023,
                [YearLevelValue.Reception, YearLevelValue.Year1]);
            var termPlan = TermPlan.Create(termPlanner, 1, []);

            termPlanner.AddTermPlan(termPlan);

            var subject = CurriculumSubject.Create("English", [], "");
            var yearLevel = YearLevel.Create(YearLevelValue.Reception, "Description");
            subject.AddYearLevel(yearLevel);

            termPlan.AddSubject(subject);

            databaseContext.TermPlanners.Add(termPlanner);

            // if there are any change tracking issues, uncomment this
            //databaseContext.TermPlanners.AsNoTracking();

            await databaseContext.SaveChangesAsync();
        }

        return databaseContext;
    }
}