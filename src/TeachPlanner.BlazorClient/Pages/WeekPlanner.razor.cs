using System.Net.Http.Json;
using TeachPlanner.BlazorClient.Models.WeekPlanner;
using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.BlazorClient.Pages;

public partial class WeekPlanner
{
    private string? _errorMessage;
    private string _gridRows = string.Empty;
    private int Year { get; set; }
    private int WeekNumber { get; set; }
    private List<TemplatePeriod> Periods { get; set; } = [];
    private List<DayPlan> DayPlans { get; set; } = [];
    private List<DayTemplate> DayTemplates { get; set; } = [];
    public int CurrentTerm { get; set; } = 1;
    public DateTime? SelectedDate { get; set; }
    private List<TermDate> TermDates { get; set; } = [];
    private DateOnly WeekStart { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (!AppState.Teacher.AccountSetupComplete)
        {
            NavManager.NavigateTo("/account-setup");
        }

        var client = HttpFactory.CreateClient("ServerApi");
        Year = AppState.Teacher.LastSelectedYear;
        WeekStart = AppState.Teacher.LastSelectedWeekStart;

        try
        {
            var termDates =
                await client.GetFromJsonAsync<IEnumerable<TermDate>>($"api/{AppState.Teacher.Id.Value}/curriculum/term-dates?year={Year}");
            if (termDates is null)
            {
                throw new TermDatesNotFoundException();
            }

            TermDates = termDates.ToList();
            CalculateTermAndWeekNumber(TermDates, WeekStart);
            var response =
                await client.GetAsync(
                    $"api/{AppState.Teacher.Id.Value}/week-planner?term={CurrentTerm}&week={WeekNumber}&year={Year}");

            WeekPlannerDto weekPlanner;
            if (!response.IsSuccessStatusCode)
            {
                var createWeekPlannerResponse = await client.PostAsJsonAsync(
                        $"api/{AppState.Teacher.Id.Value}/week-planner",
                        new CreateWeekPlannerRequest(WeekNumber, CurrentTerm, Year));
                
                if (!createWeekPlannerResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to create week planner");
                }
                
                weekPlanner = (await createWeekPlannerResponse.Content.ReadFromJsonAsync<WeekPlannerDto>())!;
            }
            else
            {
                weekPlanner = (await response.Content.ReadFromJsonAsync<WeekPlannerDto>())!;
            }

            WeekStart = weekPlanner.WeekStart;
            WeekNumber = weekPlanner.WeekNumber;
            DayPlans = weekPlanner.DayPlans.ConvertFromDtos(weekPlanner.WeekStart).SetDates(weekPlanner.WeekStart)
                .ToList();
            Periods = weekPlanner.WeekStructure.Periods;
            DayTemplates = weekPlanner.WeekStructure.DayTemplates.FromDtos();
            SetGridDimensions();
        }
        catch (Exception e)
        {
            _errorMessage = e.Message;
        }
    }

    private void SetGridDimensions()
    {
        var rows = "0.5fr "; // week and day header row

        foreach (var entry in Periods)
        {
            if (entry.PeriodType == PeriodType.Lesson)
            {
                rows += "minmax(100px, max-content)";
            }
            else
            {
                rows += "minmax(auto, 0.3fr)";
            }

            rows += " ";

            _gridRows = rows;
        }
    }

    private LessonPlan? GetLessonPlan(int i, int j)
    {
        if (DayPlans[i].LessonPlans.Count == 0)
        {
            return null;
        }

        return DayPlans[i].LessonPlans.FirstOrDefault(lp => lp.StartPeriod == j + 1);
    }
    
    private void CalculateTermAndWeekNumber(IEnumerable<TermDate> termDates, DateOnly weekStart)
    {
        var termDate = termDates.FirstOrDefault(td => td.StartDate <= weekStart && td.EndDate >= weekStart);
        if (termDate is null)
        {
            throw new TermDatesNotFoundException();
        }

        CurrentTerm = termDate.TermNumber;
        for (int i = 0; i < termDate.GetNumberOfWeeks(); i++)
        {
            if (termDate.StartDate.AddDays(i * 7) == weekStart)
            {
                WeekNumber = i + 1;
                break;
            }
        }
    }
}