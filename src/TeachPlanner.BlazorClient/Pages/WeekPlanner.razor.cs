using System.Net.Http.Json;
using TeachPlanner.BlazorClient.Models.WeekPlanner;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.BlazorClient.Pages;

public partial class WeekPlanner
{
    private string? errorMessage = null;
    private string gridRows = string.Empty;

    public int WeekNumber { get; set; }
    public List<TemplatePeriodModel> Periods { get; set; } = null!;
    public List<DayPlanModel> DayPlans { get; set; } = null!;
    public DayPlanTemplateModel? DayPlanTemplate { get; set; }
    public int CurrentTerm { get; set; } = 1;
    public DateTime? SelectedDate { get; set; }
    public List<TermDate> TermDates { get; set; } = new();
    public DateOnly WeekStart { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!AppState.Teacher.AccountSetupComplete)
        {
            NavManager.NavigateTo("/account-setup");
        }

        var client = HttpFactory.CreateClient("ServerApi");

        try
        {
            var termDates = await client.GetFromJsonAsync<IEnumerable<TermDate>>($"api/services/term-dates?year=2024");
            if (termDates is null) throw new TermDatesNotFoundException();
            TermDates = termDates.ToList();

            var weekPlanner = await client.GetFromJsonAsync<WeekPlannerDto>($"api/{AppState.Teacher.Id.Value}/week-planner?term=1&week=1&year=2024");
            if (weekPlanner is null)
            {
                throw new Exception("Failed to retrieve week planner");
            }

            WeekStart = weekPlanner.WeekStart;
            WeekNumber = weekPlanner.WeekNumber;
            DayPlans = weekPlanner.DayPlans.ConvertFromDtos(weekPlanner.WeekStart).SetDates(weekPlanner.WeekStart).ToList();
            Periods = weekPlanner.DayPlanPattern.Pattern.ConvertFromDtos();
            DayPlanTemplate = weekPlanner.DayPlanPattern.ConvertFromDto();
            SetGridDimensions();
        }
        catch (Exception e)
        {
            errorMessage = e.Message;
        }
    }

    private void SetGridDimensions()
    {
        var rows = "0.5fr "; // week and day header row

        for (int i = 0; i < Periods.Count; i++)
        {
            var entry = Periods[i];
            if (entry.Type == PeriodType.Lesson)
            {
                rows += "minmax(100px, max-content)";
            }
            else
            {
                rows += "minmax(auto, 0.3fr)";
            }

            rows += " ";

            gridRows = rows;
        }
    }

    private LessonPlanModel? GetLessonPlan(int i, int j)
    {
        if (DayPlans[i].LessonPlans.Count == 0)
        {
            return null;
        }

        return DayPlans[i].LessonPlans.Where(lp => lp.StartPeriod == j + 1).FirstOrDefault();
    }
}
