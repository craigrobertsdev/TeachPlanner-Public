using TeachPlanner.Shared.Domain.PlannerTemplates;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Shared.Common.Interfaces.Persistence;

public interface IPlannerTemplateRepository
{
    Task<WeekStructure?> GetById(WeekStructureId id, CancellationToken cancellationToken);
    Task<WeekStructure?> GetByTeacherId(TeacherId teacherId, CancellationToken cancellationToken);
    void Add(WeekStructure weekStructure);
}
