namespace TeachPlanner.Shared.Exceptions;

public class DuplicateCurriculumCodeException : BaseException
{
    public DuplicateCurriculumCodeException() : base("Cannot add a duplicate curriculum code", 400,
        "TermPlan.Duplicate")
    {
    }
}