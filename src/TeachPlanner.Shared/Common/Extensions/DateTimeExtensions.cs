namespace TeachPlanner.Shared.Common.Extensions;
public static class DateTimeExtensions
{
    public static string GetCalendarDate(this DateOnly date) => date.ToString("dd/MM/yyyy");
}
