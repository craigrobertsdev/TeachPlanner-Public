using TeachPlanner.Api.Interfaces.Services;
using TeachPlanner.Shared.Contracts.Curriculum;

namespace TeachPlanner.Api.Features.Curriculum;

public static class GetCurriculumSubjectNames
{
    public static IResult Endpoint(ICurriculumService curriculumService)
    {
        var curriculum = curriculumService.GetSubjectNames();
        return Results.Ok(new CurriculumSubjectsNamesResponse(curriculum));
    }
}