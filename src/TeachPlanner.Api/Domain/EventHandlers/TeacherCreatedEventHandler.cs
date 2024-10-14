using MediatR;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.Teachers.DomainEvents;
using TeachPlanner.Api.Domain.YearDataRecords;
using TeachPlanner.Api.Interfaces.Persistence;

namespace TeachPlanner.Api.Domain.EventHandlers;

public class TeacherCreatedDomainEventHandler(ApplicationDbContext context, IUnitOfWork unitOfWork)
    : INotificationHandler<TeacherCreatedDomainEvent>
{
    private readonly ApplicationDbContext _context = context;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task Handle(TeacherCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var weekStructure = WeekStructure.Create(notification.TeacherId);
        var yearData = YearData.Create(notification.TeacherId, DateTime.Now.Year, weekStructure);
        _context.YearData.Add(yearData);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}