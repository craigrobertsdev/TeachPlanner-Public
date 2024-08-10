using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.PlannerTemplates;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.YearDataRecords;

namespace TeachPlanner.Shared.Common.Interfaces.Persistence;

public interface IYearDataRepository : IRepository<YearData>
{
    Task<YearData?> GetByTeacherIdAndYear(TeacherId teacherId, int calendarYear, CancellationToken cancellationToken);
    Task<YearData?> GetById(YearDataId yearDataId, CancellationToken cancellationToken);
    Task<WeekStructureId?> GetWeekStructureId(YearDataId yearDataId, CancellationToken cancellationToken);
    Task SetInitialAccountDetails(Teacher teacher, List<YearLevelValue> yearLevelsTaught, WeekStructure weekStructure,
               int calendarYear, CancellationToken cancellationToken);
    Task<List<YearLevelValue>> GetYearLevelsTaught(TeacherId teacherId, int calendarYear, CancellationToken cancellationToken);
}