namespace TeachPlanner.Shared.Exceptions;

public class WeekStructureNotFoundException : BaseException
{
    public WeekStructureNotFoundException() : base(
        "No WeekStructure was found with the requested id",
        404,
        "PlannerTemplates.NotFound")
    {
    }
}

public class TemplatePeriodMismatchException : BaseException
{
    public TemplatePeriodMismatchException(int sentPeriodCount, int requiredPeriodCount) : base(
        $"{sentPeriodCount} periods were sent but {requiredPeriodCount} were required",
        400,
        "PlannerTemplates.BadRequest")
    {
    }
}