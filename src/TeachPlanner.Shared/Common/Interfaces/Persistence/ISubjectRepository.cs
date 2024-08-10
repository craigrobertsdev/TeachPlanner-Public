using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Shared.Common.Interfaces.Persistence;

public interface ISubjectRepository
{
    Task<List<CurriculumSubject>> GetCurriculumSubjects(bool includeElaborations, CancellationToken cancellationToken);

    Task<List<CurriculumSubject>> GetSubjectsById(List<SubjectId> subjects, bool includeElaborations,
        CancellationToken cancellationToken);
}