using TeachPlanner.Shared.Common.Extensions;
using TeachPlanner.Shared.Contracts.WeekPlanners;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public record DayPlanModel
{
    public List<LessonPlanModel> LessonPlans { get; set; } = [];
    public List<SchoolEventModel> SchoolEvents { get; set; } = [];
    public DateOnly Date { get; set; }
}

public static class DayPlanModelExtensions
{
    public static IEnumerable<DayPlanModel> SetDates(this IEnumerable<DayPlanModel> dayPlanModels, DateOnly weekStart) =>
        dayPlanModels.Select((dpm, i) => dpm with { Date = weekStart.AddDays(i) });

    public static IEnumerable<DayPlanModel> ConvertFromDtos(this IEnumerable<DayPlanDto> dayPlans, DateOnly weekStart)
    {
        DayPlanModel[] dayPlanModels = Enumerable.Empty<DayPlanModel>().Fill(5).ToArray();
        for (int i = 0; i < dayPlanModels.Length; i++)
        {
            dayPlanModels[i] = new DayPlanModel { Date = weekStart.AddDays(i) };
        }

        return dayPlanModels.Select(dayPlanModel => dayPlanModel with
        {
            LessonPlans = dayPlans.FirstOrDefault(dp => dp.Date == dayPlanModel.Date)?.LessonPlans.ConvertFromDtos().ToList() ?? new(),
            SchoolEvents = dayPlans.FirstOrDefault(dp => dp.Date == dayPlanModel.Date)?.SchoolEvents.ConvertFromDtos().ToList() ?? new()
        });
    }
}
