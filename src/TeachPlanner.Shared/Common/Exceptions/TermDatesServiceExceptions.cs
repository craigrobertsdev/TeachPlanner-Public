namespace TeachPlanner.Shared.Common.Exceptions;
public class TermDatesNotSetException : BaseException
{
    public TermDatesNotSetException() : base("Term Dates have not been set for that year", 404, "TermDates.NotFound") { }
}
