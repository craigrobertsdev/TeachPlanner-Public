using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Interfaces.Persistence;

public interface ISubjectRepository
{
    Task<List<CurriculumSubject>> GetCurriculumSubjects(CancellationToken cancellationToken);

    Task<List<CurriculumSubject>> GetSubjectsById(List<SubjectId> subjects, CancellationToken cancellationToken);
}