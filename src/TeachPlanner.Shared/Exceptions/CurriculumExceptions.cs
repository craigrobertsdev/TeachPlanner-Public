namespace TeachPlanner.Shared.Exceptions;

public class NoSubjectsFoundException : BaseException
{
    public NoSubjectsFoundException() : base("No subjects were found in the database", 404,
        "AustralianCurriculum.NotFound")
    {
    }
}

public class StrandHasSubstrandsException : BaseException
{
    public StrandHasSubstrandsException() : base("Cannot add content descriptions to a strand that has substrands", 404,
        "AustralianCurriculum.StrandHasSubstrands")
    {
    }
}

public class AttemptedToAddNonCurriculumSubjectException : BaseException
{
    public AttemptedToAddNonCurriculumSubjectException(string subjectName)
        : base($"Cannot add non-curriculum subjects when parsing the curriculum. Subject name: {subjectName}",
            400, "AustralianCurriculum.AddNonCurriculumSubject")
    {
    }
}