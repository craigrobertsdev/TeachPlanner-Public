using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Database.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly ApplicationDbContext _context;

    public SubjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CurriculumSubject>> GetCurriculumSubjects(
        CancellationToken cancellationToken)
    {
        return await GetSubjects(cancellationToken);
    }

    public async Task<List<CurriculumSubject>> GetSubjectsById(
        List<SubjectId> subjects,
        CancellationToken cancellationToken)
    {
        Expression<Func<CurriculumSubject, bool>> filter = s => subjects.Contains(s.Id);

        return await GetSubjects(cancellationToken, filter);
    }

    private async Task<List<CurriculumSubject>> GetSubjects(
        CancellationToken cancellationToken,
        Expression<Func<CurriculumSubject, bool>>? filter = null)
    {
        var subjectsQuery = _context.CurriculumSubjects
            .AsNoTracking();

        if (filter != null)
        {
            subjectsQuery = subjectsQuery.Where(filter);
        }

        subjectsQuery = subjectsQuery
            .Include(s => s.YearLevels)
            .ThenInclude(yl => yl.ConceptualOrganisers)
            .ThenInclude(s => s.ContentDescriptions)
            .Include(s => s.YearLevels)
            .ThenInclude(yl => yl.Dispositions)
            .Include(s => s.YearLevels)
            .ThenInclude(yl => yl.Capabilities);

        var subjects = await subjectsQuery.ToListAsync(cancellationToken);

        if (subjects.Count == 0)
        {
            throw new NoSubjectsFoundException();
        }

        return subjects;
    }
}