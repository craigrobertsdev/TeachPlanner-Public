using TeachPlanner.Api.Domain.WeekPlanners;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Helpers.Domain;

internal static class WeekPlannerHelpers
{
    internal static WeekPlanner CreateWeekPlanner(YearDataId yearDataId, int year)
    {
        return WeekPlanner.Create(yearDataId, 1, 1, year, new DateOnly(year, 1, 1));
    }
}