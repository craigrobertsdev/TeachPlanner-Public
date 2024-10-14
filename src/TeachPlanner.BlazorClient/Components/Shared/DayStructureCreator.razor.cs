using Microsoft.AspNetCore.Components;
using TeachPlanner.BlazorClient.Pages.Account;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.BlazorClient.Components.Shared;

public partial class DayStructureCreator
{
    private int[] numberOfBreaks = [1, 2, 3];
    private int[] numberOfLessons = [5, 6, 7];
    private int selectedBreakNumber = 2;
    private int selectedLessonNumber = 6;

    [Parameter] public AccountSetup Parent { get; set; } = default!;

    private void HandleLessonNumberChange(int value)
    {
        selectedLessonNumber = value;

        if (Parent.LessonTemplates.Count < value)
        {
            for (var i = Parent.LessonTemplates.Count; i < value; i++)
            {
                Parent.LessonTemplates.Add(new TemplatePeriod(PeriodType.Lesson, null, new TimeOnly(0, 0, 0),
                    new TimeOnly(0, 50, 0)));
            }
        }
        else if (Parent.LessonTemplates.Count > value)
        {
            for (var i = Parent.LessonTemplates.Count; i > value; i--)
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
            for (var i = Parent.BreakTemplates.Count; i < value; i++)
            {
                Parent.BreakTemplates.Add(new TemplatePeriod(PeriodType.Break, "Break Name", new TimeOnly(0, 0, 0),
                    new TimeOnly(0, 30, 0)));
            }
        }
        else if (Parent.BreakTemplates.Count > value)
        {
            for (var i = Parent.BreakTemplates.Count; i > value; i--)
            {
                Parent.BreakTemplates.RemoveAt(i - 1);
            }
        }
    }
}