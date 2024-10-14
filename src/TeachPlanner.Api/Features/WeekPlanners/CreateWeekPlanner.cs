using MediatR;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.WeekPlanners;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Interfaces.Services;
using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.WeekPlanners;

public static class CreateWeekPlanner
{
    public static async Task<IResult> Endpoint(Guid teacherId, CreateWeekPlannerRequest request, ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new Command(
            new TeacherId(teacherId),
            request.WeekNumber,
            request.TermNumber,
            request.Year);

        var result = await sender.Send(command, cancellationToken);

        return Results.Ok(result);
    }

    public record Command(
        TeacherId TeacherId,
        int WeekNumber,
        int TermNumber,
        int Year) : IRequest<WeekPlannerDto>;

    public sealed class Handler : IRequestHandler<Command, WeekPlannerDto>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWeekPlannerRepository _weekPlannerRepository;
        private readonly IYearDataRepository _yearDataRepository;
        private readonly ITermDatesService _termDatesService;

        public Handler(IWeekPlannerRepository weekPlannerRepository, ITeacherRepository teacherRepository,
            IUnitOfWork unitOfWork, IYearDataRepository yearDataRepository, ITermDatesService termDatesService)
        {
            _weekPlannerRepository = weekPlannerRepository;
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
            _yearDataRepository = yearDataRepository;
            _termDatesService = termDatesService;
        }

        public async Task<WeekPlannerDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetById(request.TeacherId, cancellationToken);
            if (teacher is null)
            {
                throw new TeacherNotFoundException();
            }

            var yearData = await _yearDataRepository.GetByTeacherIdAndYear(teacher.Id, request.Year, cancellationToken);
            if (yearData is null)
            {
                throw new YearDataNotFoundException();
            }

            var weekPlanner = await 
                _weekPlannerRepository.GetWeekPlanner(yearData.Id, request.WeekNumber, request.TermNumber, request.Year, cancellationToken);

            if (weekPlanner is not null)
            {
                throw new WeekPlannerAlreadyExistsException();
            }

            var weekStart = _termDatesService.GetWeekStart(request.Year, request.TermNumber, request.WeekNumber);

            weekPlanner = WeekPlanner.Create(
                yearData.Id,
                request.WeekNumber,
                request.TermNumber,
                request.Year,
                weekStart);

            _weekPlannerRepository.Add(weekPlanner);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return weekPlanner.ToDto(yearData.WeekStructure, [], []);
        }
    }
}