using TeachPlanner.Shared.Contracts.Assessments;
using TeachPlanner.Shared.Enums;

namespace TeachPlanner.BlazorClient.Models.Assessments;

public class Assessment
{
    public YearLevelValue YearLevel { get; set; }
    public string PlanningNotes { get; set; } = string.Empty;
}

public static class AssessmentExtensions
{
    public static IEnumerable<Assessment> ConvertFromDtos(this IEnumerable<AssessmentDto> assessments)
    {
        return assessments.Select(a => new Assessment { PlanningNotes = a.PlanningNotes, YearLevel = a.YearLevel });
    }
}