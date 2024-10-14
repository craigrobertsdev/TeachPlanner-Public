using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Interfaces.Persistence;

public interface IPlannerTemplateRepository
{
    Task<WeekStructure?> GetById(WeekStructureId id, CancellationToken cancellationToken);
    Task<WeekStructure?> GetByTeacherId(TeacherId teacherId, CancellationToken cancellationToken);
    void Add(WeekStructure weekStructure);
}