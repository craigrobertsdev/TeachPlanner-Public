using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Database.Repositories;

public class PlannerTemplateRepository : IPlannerTemplateRepository
{
    private readonly ApplicationDbContext _context;

    public PlannerTemplateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WeekStructure?> GetById(WeekStructureId id, CancellationToken cancellationToken)
    {
        return await _context.WeekStructures
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WeekStructure?> GetByTeacherId(TeacherId teacherId, CancellationToken cancellationToken)
    {
        var weekStructure = await _context.WeekStructures
            .Where(dp => dp.TeacherId == teacherId)
            .Include(dp => dp.Periods)
            .Include(dp => dp.DayTemplates)
            .ThenInclude(dt => dt.Lessons)
            .FirstOrDefaultAsync(cancellationToken);

        return weekStructure;
    }

    public void Add(WeekStructure weekStructure)
    {
        _context.Add(weekStructure);
    }
}