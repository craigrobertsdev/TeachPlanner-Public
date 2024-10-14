using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Api.Interfaces.Services;

public interface ITermDatesService
{
    IReadOnlyDictionary<int, IEnumerable<TermDate>> TermDatesByYear { get; }
    IReadOnlyDictionary<int, Dictionary<int, int>> TermWeekNumbers { get; }
    void SetTermDates(int year, List<TermDate> termDates);
    DateOnly GetWeekStart(int year, int termNumber, int weekNumber);
    int GetTermNumber(DateOnly date);
    int GetWeekNumber(int year, int termNumber, DateOnly weekStart);
}