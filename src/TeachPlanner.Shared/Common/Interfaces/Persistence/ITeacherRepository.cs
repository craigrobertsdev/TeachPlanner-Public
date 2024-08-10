using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.YearDataRecords;

namespace TeachPlanner.Shared.Common.Interfaces.Persistence;

public interface ITeacherRepository
{
    void Add(Teacher teacher);
    Task<Teacher?> GetByEmail(string email, CancellationToken cancellationToken);
    Task<Teacher?> GetByUserId(string userId, CancellationToken cancellationToken);
    Task<Teacher?> GetById(TeacherId teacherId, CancellationToken cancellationToken);
    Task<Teacher?> GetWithResources(TeacherId teacherId, CancellationToken cancellationToken);
    Task<Teacher?> GetByIdWithResources(TeacherId teacherId, IEnumerable<ResourceId> resources,
               CancellationToken cancellationToken);
    Task<List<Resource>> GetResourcesBySubject(TeacherId teacherId, SubjectId subjectId,
        CancellationToken cancellationToken);
    Task<List<Resource>> GetResourcesById(IEnumerable<ResourceId> resourceIds,
               CancellationToken cancellationToken);
    Task<List<CurriculumSubject>> GetSubjectsTaughtByTeacherWithoutElaborations(TeacherId teacherId,
        CancellationToken cancellationToken);
    Task<List<CurriculumSubject>> GetSubjectsTaughtByTeacherWithElaborations(TeacherId teacherId,
        CancellationToken cancellationToken);
    Task<YearData?> GetYearDataByYear(TeacherId teacherId, int calendarYear,
        CancellationToken cancellationToken);
    void Delete(Teacher teacher);
}