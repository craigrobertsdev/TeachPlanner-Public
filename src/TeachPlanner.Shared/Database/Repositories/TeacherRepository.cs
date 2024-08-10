using Microsoft.EntityFrameworkCore;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.YearDataRecords;
using TeachPlanner.Shared.Common.Interfaces.Persistence;

namespace TeachPlanner.Shared.Database.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly ApplicationDbContext _context;

    public TeacherRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Teacher teacher)
    {
        _context.Teachers.Add(teacher);
    }

    public Task<Teacher?> GetByEmail(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Teacher?> GetById(TeacherId teacherId, CancellationToken cancellationToken)
    {
        return await _context.Teachers
            .Where(t => t.Id == teacherId)
            .Include(t => t.YearDataHistory)
            .Include(t => t.SubjectsTaught)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Teacher?> GetByIdWithResources(TeacherId teacherId, IEnumerable<ResourceId> resources, CancellationToken cancellationToken)
    {
        return await _context.Teachers
            .Where(t => t.Id == teacherId)
            .Include(t => t.Resources.Where(r => resources.Contains(r.Id)))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Teacher?> GetByUserId(string userId, CancellationToken cancellationToken)
    {
        return await _context.Teachers
            .Where(t => t.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Teacher?> GetWithResources(TeacherId teacherId, CancellationToken cancellationToken)
    {
        return await _context.Teachers
            .Where(t => t.Id == teacherId)
            .Include(t => t.Resources)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Resource>> GetResourcesBySubject(TeacherId teacherId, SubjectId subjectId,
        CancellationToken cancellationToken)
    {
        var teacher = await _context.Teachers
            .Where(t => t.Id == teacherId)
            .Include(t => t.Resources
                .Where(r => r.SubjectId == subjectId))
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken);

        return teacher != null ? [.. teacher.Resources] : [];
    }

    public Task<List<CurriculumSubject>> GetSubjectsTaughtByTeacherWithElaborations(TeacherId teacherId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<CurriculumSubject>> GetSubjectsTaughtByTeacherWithoutElaborations(TeacherId teacherId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<YearData?> GetYearDataByYear(TeacherId teacherId, int calendarYear, CancellationToken cancellationToken)
    {
        return await _context.YearData
            .Where(y => y.TeacherId == teacherId && y.CalendarYear == calendarYear)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Delete(Teacher teacher)
    {
        _context.Teachers.Remove(teacher);
    }

    public async Task<List<Resource>> GetResourcesById(IEnumerable<ResourceId> resourceIds, CancellationToken cancellationToken)
    {
        return await _context.Resources
           .Where(r => resourceIds.Contains(r.Id))
           .ToListAsync(cancellationToken);
    }
}