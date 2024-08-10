using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Shared.Common.Interfaces.Services;

public interface ICurriculumService
{
    List<CurriculumSubject> CurriculumSubjects { get; }
    string GetSubjectName(SubjectId subjectId);
    List<string> GetSubjectNames();
    List<CurriculumSubject> GetSubjectsByYearLevel(YearLevelValue yearLevel);
    List<CurriculumSubject> GetSubjectsByYearLevels(IEnumerable<CurriculumSubject> subjects, IEnumerable<YearLevelValue> yearLevelValues);
    Dictionary<YearLevelValue, List<ContentDescription>> GetContentDescriptions(SubjectId subjectId, List<YearLevelValue> yearLevels);
}