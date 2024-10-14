using TeachPlanner.Api.Database;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.Dev;

public static class CreateResourceDataForTesting
{
    public static async Task<IResult> Endpoint(ApplicationDbContext context, Guid teacherId, Guid mathsId,
        Guid englishId, Guid hassId, Guid scienceId, Guid peId)
    {
        var teacher = new TeacherId(teacherId);
        var maths = new SubjectId(mathsId);
        var english = new SubjectId(englishId);
        var hass = new SubjectId(hassId);
        var science = new SubjectId(scienceId);
        var pe = new SubjectId(peId);
        var resources = new List<Resource>
        {
            Resource.Create(teacher, "Exploring Quantum Mechanics: The World of Subatomic Particles",
                "http://test.com", true, maths, [YearLevelValue.Reception, YearLevelValue.Year1], []),
            Resource.Create(teacher,
                "The origins of the universe, the stars and the earth and other things to make this a really long title",
                "http://test.com", false, maths, [YearLevelValue.Reception, YearLevelValue.Year1], []),
            Resource.Create(teacher, "Algebra 1", "http://test.com", true, maths,
                [YearLevelValue.Year1, YearLevelValue.Year2], []),
            Resource.Create(teacher, "Algebra for upper primary", "http://test.com", false, hass,
                [YearLevelValue.Year5, YearLevelValue.Year6], []),
            Resource.Create(teacher, "Introduction to Astronomy: From Stars to Exoplanets", "http://test.com",
                false, hass, [YearLevelValue.Reception, YearLevelValue.Year1], []),
            Resource.Create(teacher, "Intermediate Algebra: Mastering Equations and Inequalities",
                "http://test.com", true, hass, [YearLevelValue.Year1, YearLevelValue.Year2], []),
            Resource.Create(teacher, "Chemistry Essentials for Middle School", "http://test.com", false, science,
                [YearLevelValue.Year5, YearLevelValue.Year6], []),
            Resource.Create(teacher, "Exploring Quantum Mechanics: The World of Subatomic Particles",
                "http://test.com", true, science, [YearLevelValue.Reception, YearLevelValue.Year1], []),
            Resource.Create(teacher, "Advanced Trigonometry: Applications in Real-World Scenarios",
                "http://test.com", true, science, [YearLevelValue.Year2, YearLevelValue.Year3], []),
            Resource.Create(teacher, "Introduction to Electricity and Magnetism", "http://test.com", false, english,
                [YearLevelValue.Year6], []),
            Resource.Create(teacher, "Data Analysis and Interpretation: Extracting Insights from Information",
                "http://test.com", true, english, [YearLevelValue.Year6], []),
            Resource.Create(teacher, "Ecology: Understanding Ecosystems and Biodiversity", "http://test.com", false,
                english, [YearLevelValue.Reception, YearLevelValue.Year1], []),
            Resource.Create(teacher, "Coding Fundamentals: From Algorithms to Applications", "http://test.com",
                true, pe, [YearLevelValue.Year1, YearLevelValue.Year1], []),
            Resource.Create(teacher, "Introduction to Geology: Unraveling Earth's History", "http://test.com",
                false, pe, [YearLevelValue.Year5], []),
            Resource.Create(teacher, "Artificial Intelligence Basics: Building Intelligent Systems",
                "http://test.com", true, pe, [YearLevelValue.Reception, YearLevelValue.Year1], [])
        };

        await context.Resources.AddRangeAsync(resources);
        await context.SaveChangesAsync();
        return Results.Ok();
    }
}