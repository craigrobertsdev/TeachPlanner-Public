using TeachPlanner.Shared.Domain.Common.Primatives;
using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.Shared.Domain.YearDataRecords.DomainEvents;

public record WeekStructureAddedToYearDataEvent(Guid Id, WeekStructureId WeekStructureId) : DomainEvent(Id);
