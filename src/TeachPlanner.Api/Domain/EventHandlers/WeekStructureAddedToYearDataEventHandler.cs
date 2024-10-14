using MediatR;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.Domain.WeekPlanners;
using TeachPlanner.Api.Domain.YearDataRecords.DomainEvents;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Interfaces.Services;

namespace TeachPlanner.Api.Domain.EventHandlers;

public class WeekStructureAddedToYearDataEventHandler : INotificationHandler<WeekStructureAddedToYearDataEvent>
{
    private readonly ApplicationDbContext _context;
    private readonly ITermDatesService _termDatesService;
    private readonly IUnitOfWork _unitOfWork;

    public WeekStructureAddedToYearDataEventHandler(ApplicationDbContext context, IUnitOfWork unitOfWork,
        ITermDatesService termDatesService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _termDatesService = termDatesService;
    }

    public async Task Handle(WeekStructureAddedToYearDataEvent notification, CancellationToken cancellationToken)
    {
        var yearData = await _context.YearData
            .Where(yd => yd.WeekStructure != null && yd.WeekStructure.Id == notification.WeekStructureId)
            .Include(yd => yd.WeekPlanners)
            .FirstAsync(cancellationToken);

        if (yearData.WeekPlanners.Count == 0)
        {
            yearData.AddWeekPlanner(WeekPlanner.Create(
                yearData.Id,
                1,
                1,
                yearData.CalendarYear,
                _termDatesService.GetWeekStart(yearData.CalendarYear, 1, 1)));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}