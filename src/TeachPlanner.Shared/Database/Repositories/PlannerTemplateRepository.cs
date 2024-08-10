using Microsoft.EntityFrameworkCore;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Domain.PlannerTemplates;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Shared.Database.Repositories;

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
        return await _context.WeekStructures
            .Where(dp => dp.TeacherId == teacherId)
            .Include(dp => dp.Periods)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Add(WeekStructure weekStructure)
    {
        _context.Add(weekStructure);
    }
}
