using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Interfaces.Services;
using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.Curriculum;

public static class GetContentDescriptions
{
    public static async Task<IResult> Endpoint(Guid teacherId, Guid subjectId, [FromQuery] string[] yearLevels,
        ISender sender)
    {
        var query = new Query(new SubjectId(subjectId), yearLevels.Select(Enum.Parse<YearLevelValue>).ToList());
        var result = await sender.Send(query);
        return Results.Ok(result);
    }

    public record Query(SubjectId SubjectId, List<YearLevelValue> YearLevels)
        : IRequest<Dictionary<YearLevelValue, List<ContentDescriptionDto>>>;

    public sealed class Handler(ICurriculumService curriculumService)
        : IRequestHandler<Query, Dictionary<YearLevelValue, List<ContentDescriptionDto>>>
    {
        private readonly ICurriculumService _curriculumService = curriculumService;

        public async Task<Dictionary<YearLevelValue, List<ContentDescriptionDto>>> Handle(Query request,
            CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var contentDescriptions = _curriculumService.GetContentDescriptions(request.SubjectId, request.YearLevels);
            var contentDescriptionsDto = contentDescriptions.ToDictionary(
                x => x.Key,
                x => x.Value.Select(cd => cd.ToDto()).ToList());

            return contentDescriptionsDto;
        }
    }
}