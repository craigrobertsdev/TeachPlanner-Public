using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Api.Features.Teachers;

public static class GetYearLevelsTaught
{
    public static async Task<IResult> Delegate(Guid teacherId, int calendarYear, ITeacherRepository teacherRepository, CancellationToken cancellationToken)
    {
        var yearData = await teacherRepository.GetYearDataByYear(new TeacherId(teacherId), calendarYear, cancellationToken);
        return yearData is not null ?
            Results.Ok(yearData?.YearLevelsTaught)
            : Results.NotFound();
    }
}
