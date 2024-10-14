using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Domain.WeekPlanners;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Database.Repositories;

public class WeekPlannerRepository(ApplicationDbContext context) : IWeekPlannerRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<WeekPlanner?> GetWeekPlanner(YearDataId yearDataId, int weekNumber, int termNumber, int year,
        CancellationToken cancellationToken)
    {
        return await _context.WeekPlanners
            .Where(wp => wp.YearDataId == yearDataId)
            .Where(wp => wp.WeekNumber == weekNumber)
            .Where(wp => wp.TermNumber == termNumber)
            .Where(wp => wp.Year == year)
            .Include(wp => wp.DayPlans)
            .ThenInclude(dp => dp.LessonPlans)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WeekPlanner?> GetByLessonDate(DateOnly lessonDate, CancellationToken cancellationToken)
    {
        var weekPlanner = await _context.WeekPlanners
            .Where(wp =>
                lessonDate.DayNumber - wp.WeekStart.DayNumber < 5 && lessonDate.DayNumber - wp.WeekStart.DayNumber >= 0)
            .FirstOrDefaultAsync(cancellationToken);

        return weekPlanner;
    }

    public async Task<WeekPlanner?> GetByYearAndWeekNumber(int year, int weekNumber,
        CancellationToken cancellationToken)
    {
        var weekPlanner = await _context.WeekPlanners
            .Where(wp => wp.Year == year && wp.WeekNumber == weekNumber)
            .Include(wp => wp.DayPlans)
            .FirstOrDefaultAsync(cancellationToken);

        return weekPlanner;
    }

    public void Add(WeekPlanner weekPlanner)
    {
        _context.WeekPlanners.Add(weekPlanner);
    }
}