using TeachPlanner.Api.Domain.YearDataRecords;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Helpers.Domain;

internal static class YearDataHelpers
{
    public static YearData CreateYearData()
    {
        return YearData.Create(new TeacherId(Guid.NewGuid()), 2023, DayPlanTemplateHelpers.CreateDayPlanTemplate());
    }
}