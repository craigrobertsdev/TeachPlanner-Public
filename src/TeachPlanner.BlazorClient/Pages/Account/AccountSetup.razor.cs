using System.Data;
using System.Net.Http.Json;
using TeachPlanner.BlazorClient.Models.WeekPlanner;
using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Contracts.Teachers.AccountSetup;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.BlazorClient.Pages.Account;

public partial class AccountSetup
{
    public int[] CalendarYears = new int[2];
    public int SelectedCalendarYear;
    public List<string> Subjects = [];
    public List<string> SelectedSubjects = [];
    public string? ErrorMessage;
    public List<string> YearLevels = [];
    public List<string> SelectedYearLevels = [];
    protected AccountSetupStep CurrentStep = AccountSetupStep.AddingSubjects;
    public List<DayOfWeek> WeekDays = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday];
    public List<DayOfWeek> WorkDays { get; set; } = [];
    public List<TemplatePeriodModel> LessonTemplates { get; set; } = [];
    public List<TemplatePeriodModel> BreakTemplates { get; set; } = [];
    public WeekStructureModel WeekStructure { get; set; } = default!;
    public List<TemplatePeriodModel> CombinedPeriodTemplates = [];

    protected override void OnInitialized()
    {
        if (AppState.Teacher.AccountSetupComplete)
        {
            NavManager.NavigateTo("/week-planner", replace: true);
        }

        CalendarYears[0] = DateTime.Now.Year;
        CalendarYears[1] = DateTime.Now.Year + 1;
        SelectedCalendarYear = CalendarYears[0];
        InitialiseWeekStructure();
        GenerateInitialTemplates();
    }

    protected override async Task OnInitializedAsync()
    {
        var client = Http.CreateClient("ServerApi");
        ErrorMessage = null;
        var response = await client.GetAsync($"/api/{AppState.Teacher.Id.Value}/curriculum/subject-names");

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Whoops something went wrong.";
        }

        var content = await response.Content.ReadFromJsonAsync<CurriculumSubjectsNamesResponse>();
        Subjects = content?.SubjectNames!;

        var yearLevels = await client.GetFromJsonAsync<List<YearLevelValue>>($"/api/{AppState.Teacher.Id.Value}/curriculum/yearLevels");
        if (yearLevels is null)
        {
            ErrorMessage = "Whoops something went wrong.";
            return;
        }

        YearLevels.AddRange(yearLevels.Select(yl => yl.ToString()));
        StateHasChanged();
    }

    private async Task SetupAccount()
    {
        if (!SetupDataIsValid())
        {
            return;
        }

        var accountSetupRequest = new AccountSetupRequest(
            SelectedSubjects,
            SelectedYearLevels.Select(yl => yl.Replace(" ", "")).ToList(),
            new DayPlanPatternDto(
                LessonTemplates.Select(template => new DayPlanLessonTemplateDto(template.StartTime, template.EndTime)).ToList(),
                BreakTemplates.Select(template => new DayPlanBreakTemplateDto(template.Name ?? string.Empty, template.StartTime, template.EndTime)).ToList())
        );

        var response = await Http.CreateClient("ServerApi")
            .PostAsJsonAsync($"/api/{AppState.Teacher.Id.Value}/setup?calendarYear={SelectedCalendarYear}", accountSetupRequest);

        if (!response.IsSuccessStatusCode)
        {
            ErrorMessage = "Whoops something went wrong.";
        }

        await LocalStorage.SetItemAsync("AccountSetupComplete", true);
        AppState.Teacher.AccountSetupComplete = true;

        NavManager.NavigateTo("/week-planner", replace: true);
    }

    private bool WeekStructureIsInvalid()
    {
        foreach (var day in WeekStructure.DayTemplates)
        {
            if (day is not null && day.Lessons.Count != LessonTemplates.Count)
            {
                return true;
            }
        }

        return false;
    }

    private void InitialiseWeekStructure()
    {
        WeekStructure = new WeekStructureModel
        {
            Periods = [],
            DayTemplates = Enumerable.Range(0, 5).Select(_ => new DayTemplateModel { Lessons = [] }).ToArray()
        };
    }

    private bool SetupDataIsValid()
    {
        if (!LessonTimesAreValid())
        {
            return false;
        };

        if (SelectedSubjects.Count == 0)
        {
            ErrorMessage = "You must select at least one subject.";
            return false;
        }

        if (SelectedYearLevels.Count == 0)
        {
            ErrorMessage = "You must select at least one year level.";
            return false;
        }

        if (WeekStructureIsInvalid())
        {
            ErrorMessage = "All periods must be filled in.";
            return false;
        }

        return true;
    }

    private bool CanMoveToCreatingWeekStructure() =>
        SelectedSubjects.Count > 0 && SelectedYearLevels.Count > 0 && WorkDays.Count > 0;

    private void MoveToNextStep()
    {
        if (!CanMoveToCreatingWeekStructure())
        {
            ErrorMessage = "You must select at least one work day, subject and year level.";
            return;
        }

        ErrorMessage = null;
        WeekStructure.Periods = CombineLessonsAndBreaks();
        var workingDays = new int[5];
        foreach (var day in WorkDays)
        {
            var idx = (int)day - 1;
            workingDays[idx] = 1;
        }

        for (int i = 0; i < WeekStructure.DayTemplates.Length; i++)
        {
            if (workingDays[i] == 0)
            {
                WeekStructure.DayTemplates[i] = null;
            }
            // handles the case where the user goes back and adds more work days
            else if (WeekStructure.DayTemplates[i] == null)
            {
                WeekStructure.DayTemplates[i] = new();
            }
        }

        CurrentStep = CurrentStep switch
        {
            AccountSetupStep.AddingSubjects => AccountSetupStep.CreatingDayStructure,
            AccountSetupStep.CreatingDayStructure => AccountSetupStep.CreatingWeekStructure,
            _ => AccountSetupStep.AddingSubjects
        };
    }

    private void MoveBackAStep()
    {
        ErrorMessage = null;
        CurrentStep = CurrentStep switch
        {
            AccountSetupStep.CreatingDayStructure => AccountSetupStep.AddingSubjects,
            AccountSetupStep.CreatingWeekStructure => AccountSetupStep.CreatingDayStructure,
            _ => AccountSetupStep.AddingSubjects
        };
    }

    private List<TemplatePeriodModel> CombineLessonsAndBreaks() => new(LessonTemplates.Concat(BreakTemplates).OrderBy(tp => tp.StartTime));

    private bool LessonTimesAreValid()
    {
        ErrorMessage = null;
        var periodTimes = CombineLessonsAndBreaks();

        for (int i = 0; i < periodTimes.Count - 1; i++)
        {
            if (periodTimes[i].EndTime > periodTimes[i + 1].StartTime)
            {
                ErrorMessage = "Lesson and break times cannot overlap.";
                return false;
            }

            if (periodTimes[i].EndTime != periodTimes[i + 1].StartTime)
            {
                ErrorMessage = "Lesson and break times must be consecutive.";
                return false;
            }
        }

        return true;
    }

    private void GenerateInitialTemplates()
    {
        LessonTemplates = [
            new TemplatePeriodModel(
                PeriodType.Lesson,
                null,
                new TimeOnly(9, 10, 0),
                new TimeOnly(10, 0, 0)),
            new TemplatePeriodModel(
                PeriodType.Lesson,
                null,
                new TimeOnly(10, 0, 0),
                new TimeOnly(10, 50, 0)
            ),
            new TemplatePeriodModel(
                PeriodType.Lesson,
                null,
                new TimeOnly(11, 20, 0),
                new TimeOnly(12, 10, 0)
            ),
            new TemplatePeriodModel(
                PeriodType.Lesson,
                null,
                new TimeOnly(12, 10, 0),
                new TimeOnly(13, 0, 0)
            ),
            new TemplatePeriodModel(
                PeriodType.Lesson,
                null,
                new TimeOnly(13, 30, 0),
                new TimeOnly(14, 20, 0)
            ),
            new TemplatePeriodModel(
                PeriodType.Lesson,
                null,
                new TimeOnly(14, 20, 0),
                new TimeOnly(15, 10, 0)
            )
        ];

        BreakTemplates = [
            new TemplatePeriodModel(
                PeriodType.Break,
                "Recess",
                new TimeOnly(10, 50, 0),
                new TimeOnly(11, 20, 0)
            ),
            new TemplatePeriodModel(
                PeriodType.Break,
                "Lunch",
                new TimeOnly(13, 0, 0),
                new TimeOnly(13, 30, 0)
            ),
        ];
    }

    protected enum AccountSetupStep
    {
        AddingSubjects,
        CreatingDayStructure,
        CreatingWeekStructure
    }
}

