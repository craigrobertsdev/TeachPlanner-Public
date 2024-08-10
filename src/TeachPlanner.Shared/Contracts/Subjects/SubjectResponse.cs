using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Shared.Contracts.Subjects;

public record SubjectResponse(
    Guid Id,
    string Name,
    List<YearLevelDto> YearLevels)
{
    public static List<SubjectResponse> CreateCurriculumSubjectResponses(IEnumerable<CurriculumSubject> subjects,
        bool withDetails)
    {
        List<SubjectResponse> subjectResponses = new();
        subjectResponses = subjects.Select(s => new SubjectResponse(
            s.Id.Value,
            s.Name,
            CreateYearLevelResponses(s.YearLevels, withDetails))).ToList();

        return subjectResponses;
    }

    private static List<YearLevelDto> CreateYearLevelResponses(IEnumerable<YearLevel> yearLevels, bool withDetails)
    {
        var yearLevelDtos = yearLevels.Select(yl => yl.ToDto(withDetails)).ToList();

        return yearLevelDtos;
    }
}
