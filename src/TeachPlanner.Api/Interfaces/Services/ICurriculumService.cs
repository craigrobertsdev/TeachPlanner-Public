using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Interfaces.Services;

public interface ICurriculumService
{
    List<CurriculumSubject> CurriculumSubjects { get; }
    string GetSubjectName(SubjectId subjectId);
    List<string> GetSubjectNames();
    List<CurriculumSubject> GetSubjectsByYearLevel(YearLevelValue yearLevel);
    List<CurriculumSubject> GetSubjectsByName(IEnumerable<string> names);
    List<CurriculumSubject> GetSubjectsByYearLevels(IEnumerable<SubjectId> subjectIds,
        IEnumerable<YearLevelValue> yearLevelValues);

    Dictionary<YearLevelValue, List<ContentDescription>> GetContentDescriptions(SubjectId subjectId,
        List<YearLevelValue> yearLevels);
}