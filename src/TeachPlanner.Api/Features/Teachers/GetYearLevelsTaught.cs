using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.Teachers;

public static class GetYearLevelsTaught
{
    public static async Task<IResult> Endpoint(Guid teacherId, int calendarYear, ITeacherRepository teacherRepository,
        CancellationToken cancellationToken)
    {
        var yearData =
            await teacherRepository.GetYearDataByYear(new TeacherId(teacherId), calendarYear, cancellationToken);
        return yearData is not null
            ? Results.Ok(yearData?.YearLevelsTaught)
            : Results.NotFound();
    }
}