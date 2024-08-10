using MediatR;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Domain.PlannerTemplates;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.WeekPlanners;

namespace TeachPlanner.Api.Features.WeekPlanners;

public static class CreateWeekPlanner
{
    public record Command(
        TeacherId TeacherId,
        DateOnly WeekStart,
        int WeekNumber,
        int TermNumber,
        int Year) : IRequest<WeekPlanner>;

    public sealed class Handler : IRequestHandler<Command, WeekPlanner>
    {
        private readonly IWeekPlannerRepository _weekPlannerRepository;
        private readonly IYearDataRepository _yearDataRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IWeekPlannerRepository weekPlannerRepository, ITeacherRepository teacherRepository, IUnitOfWork unitOfWork, IYearDataRepository yearDataRepository)
        {
            _weekPlannerRepository = weekPlannerRepository;
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
            _yearDataRepository = yearDataRepository;
        }

        public async Task<WeekPlanner> Handle(Command request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetById(request.TeacherId, cancellationToken);
            if (teacher is null)
            {
                throw new TeacherNotFoundException();
            }

            var yearDataId = teacher.GetYearData(request.Year);
            if (yearDataId is null)
            {
                throw new YearDataNotFoundException();
            }

            var yearData = await _yearDataRepository.GetById(yearDataId, cancellationToken);
            if (yearData is null)
            {
                throw new YearDataNotFoundException();
            }

            var weekPlanner = WeekPlanner.Create(
                yearDataId,
                request.WeekNumber,
                request.TermNumber,
                request.Year,
                request.WeekStart);

            _weekPlannerRepository.Add(weekPlanner);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return weekPlanner;
        }
    }

    public static async Task<IResult> Delegate(Guid teacherId, CreateWeekPlannerRequest request, ISender sender, CancellationToken cancellationToken)
    {
        var command = new Command(
            new TeacherId(teacherId),
            request.WeekStart,
            request.WeekNumber,
            request.TermNumber,
            request.Year);

        var result = await sender.Send(command, cancellationToken);

        return Results.Ok(result);
    }
}

