using TeachPlanner.Api.Database;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.Users;
using TeachPlanner.Api.Domain.WeekPlanners;
using TeachPlanner.Api.Domain.YearDataRecords;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.IntegrationTests.Helpers;

public static class SeedData
{
    public static void PopulateTestData(ApplicationDbContext context)
    {
        context.Users.Add(new ApplicationUser { Email = "john.doe@mail.com", UserName = "john.doe@mail.com" });
        context.SaveChanges();

        var teacher = TeacherHelpers.CreateTeacher(context.Users.First().Id, 2024);
        var periods = CreateTemplatePeriods();
        var termDates = CreateTermDates();

        var dayTemplates = new List<DayTemplate>
        {
            DayTemplate.Create([new LessonTemplate("English", 1, 1), new LessonTemplate("Mathematics", 2, 3)])
        };
        var yearData =
            YearData.Create(teacher.Id, 2024,
                WeekStructure.Create(periods, dayTemplates, teacher.Id));
        List<CurriculumSubject> subjects = TestConstants.CurriculumSubjects;
        context.CurriculumSubjects.AddRange(subjects);
        teacher.AddYearData(YearDataEntry.Create(2024, yearData.Id));
        context.Teachers.Add(teacher);
        context.TermDates.AddRange(termDates);
        context.SaveChanges();

        context.YearData.Add(yearData);
        context.SaveChanges();

       context.WeekPlanners.Add(WeekPlanner.Create(yearData.Id, 1, 1, 2024, TestConstants.FirstDayOfTerm2024));
        context.SaveChanges();
    }

    private static List<TemplatePeriod> CreateTemplatePeriods()
    {
        return
        [
            new(PeriodType.Lesson, null, new TimeOnly(9, 10, 0), new TimeOnly(10, 0, 0)),
            new(PeriodType.Lesson, null, new TimeOnly(10, 0, 0), new TimeOnly(10, 50, 0)),
            new(PeriodType.Break, "Recess", new TimeOnly(10, 50, 0), new TimeOnly(11, 20, 0)),
            new(PeriodType.Lesson, null, new TimeOnly(11, 20, 0), new TimeOnly(12, 10, 0)),
            new(PeriodType.Lesson, null, new TimeOnly(12, 10, 0), new TimeOnly(13, 0, 0)),
            new(PeriodType.Break, "Lunch", new TimeOnly(13, 0, 0), new TimeOnly(13, 30, 0)),
            new(PeriodType.Lesson, null, new TimeOnly(13, 30, 0), new TimeOnly(14, 20, 0)),
            new(PeriodType.Lesson, null, new TimeOnly(14, 20, 0), new TimeOnly(15, 10, 0))
        ];
    }

    private static TermDate[] CreateTermDates()
    {
        // convert the json below to a TermDate object
        return
        [
            new TermDate(1, new DateOnly(2024, 1, 29), new DateOnly(2024, 4, 12)),
            new TermDate(2, new DateOnly(2024, 4, 29), new DateOnly(2024, 7, 5)),
            new TermDate(3, new DateOnly(2024, 7, 22), new DateOnly(2024, 9, 27)),
            new TermDate(4, new DateOnly(2024, 10, 14), new DateOnly(2024, 12, 13))
        ];
    }
}