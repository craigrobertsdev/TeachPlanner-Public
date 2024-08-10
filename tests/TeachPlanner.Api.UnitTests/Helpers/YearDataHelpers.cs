using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.YearDataRecords;

namespace TeachPlanner.Api.UnitTests.Helpers;
internal static class YearDataHelpers
{
    public static YearData CreateYearData()
    {
        return YearData.Create(new TeacherId(Guid.NewGuid()), 2023, DayPlanTemplateHelpers.CreateDayPlanTemplate());
    }
}
