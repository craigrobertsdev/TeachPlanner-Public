using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Interfaces.Persistence;

public interface ICurriculumRepository
{
    Task AddCurriculum(List<CurriculumSubject> subjects, CancellationToken cancellationToken);
    Task<List<CurriculumSubject>> GetAllSubjects(CancellationToken cancellationToken);
    Task<List<CurriculumSubject>> GetSubjectsByName(List<string> subjectNames, CancellationToken cancellationToken);
    Task<List<CurriculumSubject>> GetSubjectsById(List<SubjectId> subjectIds, CancellationToken cancellationToken);

    Task<List<CurriculumSubject>> GetSubjectsByYearLevels(List<YearLevelValue> yearLevels,
        CancellationToken cancellationToken);
}