using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.YearDataRecords;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Database.Repositories;

public class YearDataRepository(ApplicationDbContext context) : IYearDataRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<YearData?> GetByTeacherIdAndYear(TeacherId teacherId, int calendarYear,
        CancellationToken cancellationToken)
    {
        return await _context.YearData
            .Where(yd => yd.TeacherId == teacherId && yd.CalendarYear == calendarYear)
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

    public async Task SetInitialAccountDetails(Teacher teacher, List<YearLevelValue> yearLevelsTaught,
        WeekStructure weekStructure,
        int calendarYear, CancellationToken cancellationToken)
    {
        var yearData = await _context.YearData
            .Where(yd => yd.CalendarYear == calendarYear && yd.TeacherId == teacher.Id)
            .Include(yd => yd.WeekStructure)
            .FirstOrDefaultAsync(cancellationToken);

        if (yearData is null)
        {
            yearData = YearData.Create(teacher.Id, calendarYear, weekStructure);
            _context.YearData.Add(yearData);
        }
        else
        {
            yearData.UpdateWeekStructure(weekStructure);
        }

        yearData.SetYearLevelsTaught(yearLevelsTaught);
    }

    public async Task<List<YearLevelValue>> GetYearLevelsTaught(TeacherId teacherId, int calendarYear,
        CancellationToken cancellationToken)
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