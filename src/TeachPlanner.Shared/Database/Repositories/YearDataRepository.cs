using Microsoft.EntityFrameworkCore;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.PlannerTemplates;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.YearDataRecords;
using TeachPlanner.Shared.Common.Interfaces.Persistence;

namespace TeachPlanner.Shared.Database.Repositories;

public class YearDataRepository(ApplicationDbContext context) : IYearDataRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<YearData?> GetByTeacherIdAndYear(TeacherId teacherId, int calendarYear,
        CancellationToken cancellationToken)
    {
        return await _context.YearData
            .Where(yd => yd.CalendarYear == calendarYear)
            .Include(yd => yd.Subjects)
            .Include(yd => yd.Students)
            .Include(yd => yd.WeekPlanners)
            .Include(yd => yd.LessonPlans)
            .Include(yd => yd.WeekStructure)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<YearData?> GetById(YearDataId yearDataId, CancellationToken cancellationToken)
    {
        return await _context.YearData
            .Where(yd => yd.Id == yearDataId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task SetInitialAccountDetails(Teacher teacher, List<YearLevelValue> yearLevelsTaught, WeekStructure weekStructure,
               int calendarYear, CancellationToken cancellationToken)
    {
        var yearData = await _context.YearData
            .Where(yd => yd.CalendarYear == calendarYear)
            .Include(yd => yd.WeekStructure)
            .FirstAsync(cancellationToken);

        yearData.SetWeekStructure(weekStructure);
        yearData.SetYearLevelsTaught(yearLevelsTaught);
    }

    public async Task<List<YearLevelValue>> GetYearLevelsTaught(TeacherId teacherId, int calendarYear, CancellationToken cancellationToken)
    {
        var yearData = await _context.YearData
            .Where(yd => yd.CalendarYear == calendarYear)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return yearData?.YearLevelsTaught.ToList() ?? [];
    }

    public async Task<WeekStructureId?> GetWeekStructureId(YearDataId yearDataId, CancellationToken cancellationToken)
    {
        var yearData = await _context.YearData
            .Where(yd => yd.Id == yearDataId)
            .Include(yd => yd.WeekStructure)
            .FirstOrDefaultAsync(cancellationToken);

        if (yearData is null)
        {
            return null;
        }

        return yearData.WeekStructure.Id;
    }
}