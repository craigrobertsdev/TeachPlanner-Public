using MediatR;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.PlannerTemplates;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.Features.PlannerTemplates;

/// <summary>
///     Handles any time periods are added to a WeekStructure. <br />
///     This will always be called on user creation as a blank WeekStructure is created at the time of Teacher creation,
///     and the user will need to add their template periods before starting to use the WeekPlanner
/// </summary>
public static class UpdateWeekStructure
{
    public static async Task<IResult> Endpoint(Guid teacherId, WeekStructureRequest request, int calendarYear,
        ISender sender, CancellationToken cancellationToken)
    {
        var command = new Command(
            request.Periods.Select(
                period => new TemplatePeriod(
                    Enum.Parse<PeriodType>(period.PeriodType),
                    period.Name ?? string.Empty,
                    period.StartTime,
                    period.EndTime)).ToList(),
            new TeacherId(teacherId),
            calendarYear);

        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }

    public record Command(List<TemplatePeriod> Periods, TeacherId TeacherId, int CalendarYear) : IRequest;

    public sealed class Handler(
        IPlannerTemplateRepository plannerTemplateRepository,
        IUnitOfWork unitOfWork,
        ITeacherRepository teacherRepository,
        IYearDataRepository yearDataRepository) : IRequestHandler<Command>
    {
        private readonly IPlannerTemplateRepository _plannerTemplateRepository = plannerTemplateRepository;
        private readonly ITeacherRepository _teacherRepository = teacherRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IYearDataRepository _yearDataRepository = yearDataRepository;

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetById(request.TeacherId, cancellationToken);
            if (teacher is null)
            {
                throw new TeacherNotFoundException();
            }

            var yearDataId = teacher.GetYearData(request.CalendarYear);
            if (yearDataId is null)
            {
                throw new YearDataNotFoundException();
            }

            var weekStructureId = await _yearDataRepository.GetWeekStructureId(yearDataId, cancellationToken);
            if (weekStructureId is null)
            {
                throw new WeekStructureNotFoundException();
            }

            var weekStructure = await _plannerTemplateRepository.GetById(weekStructureId, cancellationToken);
            if (weekStructure is null)
            {
                throw new WeekStructureNotFoundException();
            }

            if (request.Periods.Count != weekStructure.Periods.Count)
            {
                throw new TemplatePeriodMismatchException(request.Periods.Count, weekStructure.Periods.Count);
            }

            weekStructure.SetPeriods(request.Periods);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}