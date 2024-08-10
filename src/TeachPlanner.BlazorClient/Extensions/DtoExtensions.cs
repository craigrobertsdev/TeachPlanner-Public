using TeachPlanner.BlazorClient.Models.Assessments;
using TeachPlanner.Shared.Contracts.Assessments;
using TeachPlanner.Shared.Contracts.Services;
using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.BlazorClient.Extensions;
public static class DtoExtensions
{
    #region TermDateDto

    public static IEnumerable<TermDate> ConvertFromDtos(this IEnumerable<TermDateDto> termDates) =>
        termDates.Select((td, i) => new TermDate(i + 1, GetDateFromString(td.StartDate), GetDateFromString(td.EndDate)));

    public static IEnumerable<TermDateDto> ToDtos(this IEnumerable<TermDate> termDates) =>
        termDates.Select(td => new TermDateDto(td.StartDate.ToString("yyyy-MM-dd"), td.EndDate.ToString("yyyy-MM-dd")));

    #endregion

    #region AssessmentDto
    public static IEnumerable<AssessmentModel> ConvertFromDtos(this IEnumerable<AssessmentDto> assessments) =>
        assessments.Select(a => new AssessmentModel
        {
            PlanningNotes = a.PlanningNotes,
            YearLevel = a.YearLevel
        });

    #endregion

    #region Helpers
    private static DateOnly GetDateFromString(string date)
    {
        var dateParts = date.Split('-');
        return new DateOnly(int.Parse(dateParts[0]), int.Parse(dateParts[1]), int.Parse(dateParts[2]));
    }
    #endregion
}
