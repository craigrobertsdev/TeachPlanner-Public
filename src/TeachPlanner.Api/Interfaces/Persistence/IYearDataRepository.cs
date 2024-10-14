using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.YearDataRecords;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Interfaces.Persistence;

public interface IYearDataRepository : IRepository<YearData>
{
    Task<YearData?> GetByTeacherIdAndYear(TeacherId teacherId, int calendarYear, CancellationToken cancellationToken);
    Task<YearData?> GetById(YearDataId yearDataId, CancellationToken cancellationToken);
    Task<WeekStructureId?> GetWeekStructureId(YearDataId yearDataId, CancellationToken cancellationToken);

    Task SetInitialAccountDetails(Teacher teacher, List<YearLevelValue> yearLevelsTaught, WeekStructure weekStructure,
        int calendarYear, CancellationToken cancellationToken);

    Task<List<YearLevelValue>> GetYearLevelsTaught(TeacherId teacherId, int calendarYear,
        CancellationToken cancellationToken);
}