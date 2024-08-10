using MediatR;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Database;
using TeachPlanner.Shared.Domain.PlannerTemplates;
using TeachPlanner.Shared.Domain.Teachers.DomainEvents;
using TeachPlanner.Shared.Domain.YearDataRecords;

namespace TeachPlanner.Shared.Domain.EventHandlers;

public class TeacherCreatedDomainEventHandler(ApplicationDbContext context, IUnitOfWork unitOfWork) : INotificationHandler<TeacherCreatedDomainEvent>
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