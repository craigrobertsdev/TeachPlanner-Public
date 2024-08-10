using Microsoft.EntityFrameworkCore;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Domain.LessonPlans;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.YearDataRecords;

namespace TeachPlanner.Shared.Database.Repositories;

public class LessonPlanRepository(ApplicationDbContext context) : ILessonPlanRepository
{
    private readonly ApplicationDbContext _context = context;

    public void Add(LessonPlan lessonPlan)
    {
        _context.Add(lessonPlan);
    }

    public async Task<List<LessonPlan>> GetByYearDataAndDate(YearDataId yearDataId, DateOnly date,
        CancellationToken cancellationToken)
    {
        return await _context.LessonPlans
            .Where(lp => lp.YearDataId == yearDataId)
            .Where(lp => lp.LessonDate == date)
            .Include(lp => lp.Resources)
            .ToListAsync(cancellationToken);
    }

    public async Task<LessonPlan?> GetByYearDataAndDateAndPeriod(YearDataId yearDataId, DateOnly date, int period, CancellationToken cancellationToken)
    {
        return await _context.LessonPlans
            .Where(lp => lp.YearDataId == yearDataId)
            .Where(lp => lp.LessonDate == date)
            .Where(lp => lp.StartPeriod == period)
            .Include(lp => lp.Resources)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void UpdateLessonPlan(LessonPlan lessonPlan)
    {
        _context.Update(lessonPlan);
    }

    public async Task<List<LessonPlan>?> GetLessonsByYearDataId(YearDataId yearDataId,
        CancellationToken cancellationToken)
    {
        return await _context.LessonPlans
            .Where(lessonPlan => lessonPlan.YearDataId == yearDataId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LessonPlan>> GetByDate(YearDataId yearDataId, DateOnly date, CancellationToken cancellationToken)
    {
        return await _context.LessonPlans
            .Where(lp => lp.YearDataId == yearDataId)
            .Where(lp => lp.LessonDate == date)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Resource>> GetResources(LessonPlan lessonPlan, CancellationToken cancellationToken)
    {
        return await _context.Resources
            .Where(r => lessonPlan.Resources.ToList().Contains(r))
            .ToListAsync(cancellationToken);
    }
    public void DeleteLessonPlans(IEnumerable<LessonPlan> lessonPlans)
    {
        foreach (var lessonPlan in lessonPlans)
        {
            lessonPlan.ClearResources();
        }

        _context.RemoveRange(lessonPlans);

    }
}