using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.Teachers.DomainEvents;

public record TeacherCreatedDomainEvent(Guid Id, TeacherId TeacherId) : DomainEvent(Id);