using TeachPlanner.Api.Domain.WeekPlanners;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Interfaces.Persistence;

public interface IWeekPlannerRepository
{
    Task<WeekPlanner?> GetWeekPlanner(YearDataId yearDataId, int weekNumber, int termNumber, int year,
        CancellationToken cancellationToken);

    Task<WeekPlanner?> GetByLessonDate(DateOnly lessonDate, CancellationToken cancellationToken);
    Task<WeekPlanner?> GetByYearAndWeekNumber(int year, int weekNumber, CancellationToken cancellationToken);
    void Add(WeekPlanner weekPlanner);
}