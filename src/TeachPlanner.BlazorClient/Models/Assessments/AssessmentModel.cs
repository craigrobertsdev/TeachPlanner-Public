using TeachPlanner.Shared.Contracts.Assessments;
using TeachPlanner.Shared.Domain.Common.Enums;

namespace TeachPlanner.BlazorClient.Models.Assessments;

public class AssessmentModel
{
    public YearLevelValue YearLevel { get; set; }
    public string PlanningNotes { get; set; } = string.Empty;
}
public static class AssessmentModelExtensions
{
    public static IEnumerable<AssessmentModel> ConvertFromDtos(this IEnumerable<AssessmentDto> assessments) =>
        assessments.Select(a => new AssessmentModel
        {
            PlanningNotes = a.PlanningNotes,
            YearLevel = a.YearLevel
        });
}
