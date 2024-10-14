using TeachPlanner.Api.Domain.Curriculum;

namespace TeachPlanner.Api.Interfaces.Curriculum;

public interface ICurriculumParser
{
    Task<List<CurriculumSubject>> ParseCurriculum();
}