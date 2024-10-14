using MediatR;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.TermPlanners;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.TermPlanners;

public static class GetTermPlanner
{
    public static async Task<IResult> Endpoint(Guid teacherId, int calendarYear, ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new Query(new TeacherId(teacherId), calendarYear);

        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }

    public record Query(TeacherId TeacherId, int CalendarYear) : IRequest<TermPlannerDto>;

    public class Handler : IRequestHandler<Query, TermPlannerDto>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ITermPlannerRepository _termPlannerRepository;

        public Handler(ITermPlannerRepository termPlannerRepository, ITeacherRepository teacherRepository)
        {
            _termPlannerRepository = termPlannerRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<TermPlannerDto> Handle(Query request, CancellationToken cancellationToken)
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

            var termPlanner =
                await _termPlannerRepository.GetByYearDataIdAndYear(yearDataId, request.CalendarYear,
                    cancellationToken);

            if (termPlanner is null)
            {
                throw new TermPlannerNotFoundException();
            }

            return new TermPlannerDto();
        }
    }
}