using TeachPlanner.Shared.Domain.LessonPlans;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.YearDataRecords;

namespace TeachPlanner.Shared.Common.Interfaces.Persistence;

public interface ILessonPlanRepository
{
    void Add(LessonPlan lesson);
    Task<List<LessonPlan>?> GetLessonsByYearDataId(YearDataId yearDataId, CancellationToken cancellationToken);
    Task<List<Resource>> GetResources(LessonPlan lessonPlan, CancellationToken cancellationToken);
    Task<List<LessonPlan>> GetByYearDataAndDate(YearDataId yearDataId, DateOnly date,
        CancellationToken cancellationToken);
    Task<List<LessonPlan>> GetByDate(YearDataId yearDataId, DateOnly date, CancellationToken cancellationToken);
    void UpdateLessonPlan(LessonPlan lessonPlan);
    void DeleteLessonPlans(IEnumerable<LessonPlan> lessonPlans);

    Task<LessonPlan?> GetByYearDataAndDateAndPeriod(YearDataId yearDataId, DateOnly date, int period, CancellationToken cancellationToken);
}