namespace TeachPlanner.Api.Services.CurriculumParser.SACurriculum;

public class MathematicsParser : BaseParser
{
    private static readonly char[] _contentDescriptionEndings = ['*'];

    public MathematicsParser() : base("Mathematics", _contentDescriptionEndings)
    {
    }
}