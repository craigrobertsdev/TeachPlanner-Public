using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.YearDataRecords.DomainEvents;

public record WeekStructureAddedToYearDataEvent(Guid Id, WeekStructureId WeekStructureId) : DomainEvent(Id);