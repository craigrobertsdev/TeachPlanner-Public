using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Extensions;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public record DayPlan
{
    public List<LessonPlan> LessonPlans { get; set; } = [];
    public List<SchoolEvent> SchoolEvents { get; set; } = [];
    public DateOnly Date { get; set; }
}

public static class DayPlanExtensions
{
    public static IEnumerable<DayPlan> SetDates(this IEnumerable<DayPlan> dayPlans, DateOnly weekStart)
    {
        return dayPlans.Select((dp, i) => dp with { Date = weekStart.AddDays(i) });
    }

    public static IEnumerable<DayPlan> ConvertFromDtos(this IEnumerable<DayPlanDto> dayPlanDtos, DateOnly weekStart)
    {
        var dayPlans = Enumerable.Empty<DayPlan>().Fill(5).ToArray();
        for (var i = 0; i < dayPlans.Length; i++)
        {
            dayPlans[i] = new DayPlan { Date = weekStart.AddDays(i) };
        }

        return dayPlans.Select(dayPlan => dayPlan with
        {
            LessonPlans =
            dayPlanDtos.FirstOrDefault(dp => dp.Date == dayPlan.Date)?.LessonPlans.ConvertFromDtos().ToList() ?? [],
            SchoolEvents = dayPlanDtos.FirstOrDefault(dp => dp.Date == dayPlan.Date)?.SchoolEvents.ConvertFromDtos()
                .ToList() ?? []
        });
    }
}