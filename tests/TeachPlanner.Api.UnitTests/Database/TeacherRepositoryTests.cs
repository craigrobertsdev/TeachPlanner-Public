using FakeItEasy;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.YearDataRecords;
using TeachPlanner.Api.Tests.Helpers.Domain;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.Tests.Database;

public class TeacherRepositoryTests
{
    private readonly IPublisher _publisher = A.Fake<IPublisher>();

    private async Task<ApplicationDbContext> GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TeachPlanner")
            .Options;

        var databaseContext = new ApplicationDbContext(options, _publisher);
        databaseContext.Database.EnsureDeleted();
        databaseContext.Database.EnsureCreated();

        if (!await databaseContext.Teachers.AnyAsync())
        {
            var teacher1 = Teacher.Create(Guid.NewGuid().ToString(), "John", "Smith");
            var yearData1 = YearData.Create(teacher1.Id, 2023,
                WeekStructure.Create([], teacher1.Id));
            yearData1.AddSubjects(SubjectHelpers.CreateCurriculumSubjects());
            teacher1.AddYearData(YearDataEntry.Create(2023, yearData1.Id));

            var teacher2 = Teacher.Create(Guid.NewGuid().ToString(), "Jane", "Smith");
            var yearData2 = YearData.Create(teacher2.Id, 2023,
                WeekStructure.Create([], teacher2.Id));
            yearData2.AddSubjects(SubjectHelpers.CreateCurriculumSubjects());

            databaseContext.Teachers.Add(teacher1);
            databaseContext.Teachers.Add(teacher2);
        }

        // if there are any change tracking issues, uncomment this
        //databaseContext.TermPlanners.AsNoTracking();

        await databaseContext.SaveChangesAsync();
        return databaseContext;
    }
}