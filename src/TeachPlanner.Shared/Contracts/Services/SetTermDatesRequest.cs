using TeachPlanner.Shared.Domain.PlannerTemplates;

namespace TeachPlanner.Shared.Contracts.Services;

public record SetTermDatesRequest(int CalendarYear, List<TermDate> TermDates);
