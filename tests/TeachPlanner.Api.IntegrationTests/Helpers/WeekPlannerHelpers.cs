using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.WeekPlanners;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.IntegrationTests.Helpers;

internal static class WeekPlannerHelpers
{
    private static WeekPlanner CreateWeekPlanner(YearDataId yearDataId, int year, DateOnly weekStart, int weekNumber)
    {
        return WeekPlanner.Create(yearDataId, weekNumber, 1, year, weekStart);
    }
    
    internal static async Task AddWeekPlannerToDatabase(MySqlFixture fixture, Teacher teacher, DateOnly lessonDate, int weekNumber)
    {
        await using var dbContext = fixture.CreateDbContext();
        var yearDataId = teacher.GetYearData(2024)!;
        var subject = dbContext.CurriculumSubjects.First();
        var weekPlanner = CreateWeekPlanner(yearDataId, 2024, lessonDate, weekNumber);
        var lessonPlan = LessonPlan.Create(yearDataId, subject.Id, [], "", "", 1, 1, lessonDate,
            []);
        var dayPlan = DayPlan.Create(lessonDate, weekPlanner.Id, [lessonPlan], []);
        weekPlanner.UpdateDayPlan(dayPlan);
        dbContext.WeekPlanners.Add(weekPlanner);
        await dbContext.SaveChangesAsync();
    }

    internal static async Task CreateLessonPlan(MySqlFixture fixture, Teacher teacher, int startPeriod, DateOnly lessonDate)
    {
        await using var dbContext = fixture.CreateDbContext();
        var yearDataId = teacher.GetYearData(2024)!;
        var subject = dbContext.CurriculumSubjects.First();
        var lessonPlan = LessonPlan.Create(yearDataId, subject.Id, [], "", "", 1, startPeriod, lessonDate,
            []);
        var weekPlanner = await dbContext.WeekPlanners
            .Where(wp => wp.WeekStart == lessonDate)
            .Include(wp => wp.DayPlans)
            .ThenInclude(dp => dp.LessonPlans)
            .FirstAsync();

        var dayPlan = weekPlanner.DayPlans
            .First(dp => dp.Date == lessonDate);
        dayPlan.AddLessonPlan(lessonPlan);
        dbContext.WeekPlanners.Update(weekPlanner);
        await dbContext.SaveChangesAsync();
    }
}