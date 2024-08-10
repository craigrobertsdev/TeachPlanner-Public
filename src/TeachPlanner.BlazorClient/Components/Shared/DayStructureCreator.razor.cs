using Microsoft.AspNetCore.Components;
using TeachPlanner.BlazorClient.Models.WeekPlanner;
using TeachPlanner.BlazorClient.Pages.Account;
using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.BlazorClient.Components.Shared;

public partial class DayStructureCreator
{
    private int[] numberOfLessons = { 5, 6, 7 };
    private int[] numberOfBreaks = { 1, 2, 3 };
    private int selectedLessonNumber = 6;
    private int selectedBreakNumber = 2;

    [Parameter]
    public AccountSetup Parent { get; set; } = default!;

    private void HandleLessonNumberChange(int value)
    {
        selectedLessonNumber = value;

        if (Parent.LessonTemplates.Count < value)
        {
            for (int i = Parent.LessonTemplates.Count; i < value; i++)
            {
                Parent.LessonTemplates.Add(new TemplatePeriodModel(PeriodType.Lesson, null, new TimeOnly(0, 0, 0), new TimeOnly(0, 50, 0)));
            }
        }
        else if (Parent.LessonTemplates.Count > value)
        {
            for (int i = Parent.LessonTemplates.Count; i > value; i--)
            {
                Parent.LessonTemplates.RemoveAt(i - 1);
            }
        }
    }

    private void HandleBreakNumberChange(int value)
    {
        selectedBreakNumber = value;

        if (Parent.BreakTemplates.Count < value)
        {
            for (int i = Parent.BreakTemplates.Count; i < value; i++)
            {
                Parent.BreakTemplates.Add(new TemplatePeriodModel(PeriodType.Break, "Break Name", new TimeOnly(0, 0, 0), new TimeOnly(0, 30, 0)));
            }
        }
        else if (Parent.BreakTemplates.Count > value)
        {
            for (int i = Parent.BreakTemplates.Count; i > value; i--)
            {
                Parent.BreakTemplates.RemoveAt(i - 1);
            }
        }
    }
}
