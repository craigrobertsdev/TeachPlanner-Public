using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.Resources;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.Teachers;

public static class GetResources
{
    public static async Task<IResult> Endpoint([FromRoute] Guid teacherId, [FromRoute] Guid subjectId, ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new Query(new TeacherId(teacherId), new SubjectId(subjectId));

        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }

    public record Query(TeacherId TeacherId, SubjectId SubjectId)
        : IRequest<List<ResourceDto>>;

    public sealed class Handler : IRequestHandler<Query, List<ResourceDto>>
    {
        private readonly ITeacherRepository _teacherRepository;

        public Handler(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<List<ResourceDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var resources = await _teacherRepository.GetResourcesBySubject(
                request.TeacherId,
                request.SubjectId,
                cancellationToken);

            var response = new List<ResourceDto>();
            foreach (var resource in resources)
            {
                response.Add(resource.ConvertToDto());
            }

            return response;
        }
    }
}