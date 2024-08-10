using Microsoft.AspNetCore.Components;
using TeachPlanner.BlazorClient.Models.WeekPlanner;
using TeachPlanner.BlazorClient.Pages.Account;
using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.BlazorClient.Pages;

public partial class LessonTemplateCreator
{
    [Parameter] public AccountSetup Parent { get; set; } = default!;

    private const string NIT = "NIT";
    public string? _errorMessage = null;
    public string _gridRows = string.Empty;
    public readonly string[] _daysOfTheWeek = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
    public int? NumberOfPeriods;
    public List<int> LessonSpans = [1, 2];
    public int SelectedDay;
    public int SelectedPeriod;
    public LessonTemplateModel? SelectedLesson { get; set; }

    protected override void OnInitialized()
    {
        SetGridDimensions();
        GenerateLessonPlans();
    }

    private void SetGridDimensions()
    {
        var rows = "0.5fr "; // week and day header row

        for (int i = 0; i < Parent.CombinedPeriodTemplates.Count; i++)
        {
            var entry = Parent.CombinedPeriodTemplates[i];
            if (entry.Type == PeriodType.Lesson)
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

    private void GenerateLessonPlans()
    {
        for (int i = 0; i < 5; i++)
        {
            // don't generate lesson plans for non-working days
            if (Parent.WeekStructure.DayTemplates[i] == null)
            {
                continue;
            }

            // don't generate lesson plans for days that already have lessons
            // this occurs when moving back and forth throughout the account setup process
            if (Parent.WeekStructure.DayTemplates[i]!.Lessons.Count > 0)
            {
                continue;
            }

            for (int j = 0; j < Parent.WeekStructure.Periods.Count; j++)
            {
                if (Parent.WeekStructure.Periods[j].Type != PeriodType.Lesson)
                {
                    continue;
                }

                var lessonPlan = new LessonTemplateModel
                {
                    DayOfWeek = (DayOfWeek)(i + 1),
                    Type = PeriodType.Lesson,
                    NumberOfPeriods = 1,
                    StartPeriod = j + 1,
                    SubjectName = string.Empty
                };

                Parent.WeekStructure.DayTemplates[i]!.Lessons.Add(lessonPlan);
            }
        }
    }

    private LessonTemplateModel GetLessonPlan(int col, int startPeriod)
    {
        return Parent.WeekStructure.DayTemplates[col]!.Lessons.Where(lp => lp.StartPeriod == startPeriod).First();
    }

    private bool IsSelected(LessonTemplateModel lesson)
    {
        if (SelectedLesson is null)
        {
            return false;
        }

        return lesson.DayOfWeek == SelectedLesson.DayOfWeek && lesson.StartPeriod == SelectedLesson.StartPeriod;
    }

    private void SelectLesson(LessonTemplateModel lessonPlan, int day, int period)
    {
        SelectedDay = day;
        SelectedPeriod = CalculatePeriodNumber(period);

        if (IsSelected(lessonPlan))
        {
            SelectedLesson = null;
            return;
        }

        SelectedLesson = lessonPlan;
    }

    private int CalculatePeriodNumber(int period)
    {
        var breakIndexes = Parent.WeekStructure.Periods.FindAll(p => p.Type == PeriodType.Break).Select(p => Parent.WeekStructure.Periods.IndexOf(p));
        var periodAdjustmentCount = 0;
        foreach (var idx in breakIndexes)
        {
            if (period > idx)
            {
                periodAdjustmentCount++;
            }
        }

        return period - periodAdjustmentCount;
    }

    public void SelectSubject(string subject)
    {
        SelectedLesson!.SubjectName = subject;

        if (subject == NIT)
        {
            SelectedLesson.Type = PeriodType.NIT;
        }
        else
        {
            SelectedLesson.Type = PeriodType.Lesson;
        }
    }

    public void SelectNumberOfLessons(int? numberOfLessons)
    {
        if (SelectedLesson!.NumberOfPeriods == 2 && numberOfLessons == 1)
        {
            Parent.WeekStructure.DayTemplates[SelectedDay]!.Lessons[SelectedPeriod + 1].SubjectName = string.Empty;
            Parent.WeekStructure.DayTemplates[SelectedDay]!.Lessons[SelectedPeriod + 1].NumberOfPeriods = 1;
        }

        SelectedLesson!.NumberOfPeriods = (int)numberOfLessons!;
    }
}

