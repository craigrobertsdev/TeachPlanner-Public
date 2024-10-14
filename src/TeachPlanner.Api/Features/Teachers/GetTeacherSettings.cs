using MediatR;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.Teachers;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.Teachers;

public static class GetTeacherSettings
{
    public static async Task<IResult> Endpoint(Guid teacherId, ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new Query(new TeacherId(teacherId));
        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }

    public record Query(TeacherId TeacherId) : IRequest<SettingsResponse>;

    public sealed class Handler : IRequestHandler<Query, SettingsResponse>
    {
        private readonly ITeacherRepository _teacherRepository;

        public Handler(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<SettingsResponse> Handle(Query request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetById(request.TeacherId, cancellationToken);
            if (teacher == null)
            {
                throw new TeacherNotFoundException();
            }

            return new SettingsResponse(teacher.LastSelectedYear, teacher.LastSelectedWeekStart);
        }
    }
}