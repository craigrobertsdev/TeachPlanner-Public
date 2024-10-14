using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.Interfaces.Services;
using TeachPlanner.Shared.Contracts.Services;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.Features.Services;

public static class SetTermDates
{
    public static async Task<IResult> Endpoint([FromBody] SetTermDatesRequest request, ISender sender)
    {
        var command = new Command(request.CalendarYear, request.TermDates);

        await sender.Send(command);

        return Results.Ok();
    }

    public record Command(int CalendarYear, List<TermDate> TermDates) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly ApplicationDbContext _context;
        private readonly ITermDatesService _termDateService;

        public Handler(ITermDatesService termDateService, ApplicationDbContext context)
        {
            _termDateService = termDateService;
            _context = context;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var termDates = _context.TermDates.Where(td => td.StartDate.Year == request.TermDates[0].StartDate.Year)
                .ToList();

            if (termDates.Count == 0)
            {
                _context.TermDates.AddRange(request.TermDates);
            }
            else
            {
                _context.TermDates.RemoveRange(termDates);
                _context.TermDates.AddRange(request.TermDates);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _termDateService.SetTermDates(request.CalendarYear, request.TermDates);
        }
    }
}