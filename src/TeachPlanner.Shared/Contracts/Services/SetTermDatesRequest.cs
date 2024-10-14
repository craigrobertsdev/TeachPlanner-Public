using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Shared.Contracts.Services;

public record SetTermDatesRequest(int CalendarYear, List<TermDate> TermDates);