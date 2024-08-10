namespace TeachPlanner.Shared.Common.Exceptions;

public class TooManyDayPlansInWeekPlannerException : BaseException
{
    public TooManyDayPlansInWeekPlannerException()
        : base("A week planner can only have 5 day plans", 400, "WeekPlanner.TooManyDayPlans")
    {
    }
}

public class WeekPlannerNotFoundException : BaseException
{
    public WeekPlannerNotFoundException() :
        base("Week Planner not found", 404, "WeekPlanner.NotFound")
    {
    }
}

public class TermDatesNotFoundException : BaseException
{
    public TermDatesNotFoundException() :
        base("Term dates not found", 404, "TermDates.NotFound")
    {
    }
}
