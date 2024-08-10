using Microsoft.EntityFrameworkCore;
using TeachPlanner.Shared.Common.Interfaces.Services;
using TeachPlanner.Shared.Database;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Api.Services;

/// <summary>
///     This class is responsible for loading the curriculum subjects from the database and storing them in memory.
///     Will be created as a singleton service in the DI container and be the source of truth for all curriculum subjects.
/// </summary>
public sealed class CurriculumService : ICurriculumService
{
    private readonly IServiceProvider _serviceProvider;
    public List<CurriculumSubject> CurriculumSubjects { get; } = [];

    public CurriculumService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CurriculumSubjects = Task.Run(LoadCurriculumSubjects).Result;
    }

    private async Task<List<CurriculumSubject>> LoadCurriculumSubjects()
    {
        using var scope = _serviceProvider.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var subjects = await _context.CurriculumSubjects
            .Include(c => c.YearLevels)
            .ThenInclude(yl => yl.ConceptualOrganisers)
            .ThenInclude(s => s.ContentDescriptions)
            .ToListAsync();
        return subjects;
    }

    public List<string> GetSubjectNames()
    {
        return CurriculumSubjects.Select(x => x.Name).ToList();
    }

    public List<CurriculumSubject> GetSubjectsByYearLevel(YearLevelValue yearLevel) =>
        CurriculumSubjects.Select(s => s.FilterYearLevels(yearLevel)).ToList();

    public List<CurriculumSubject> GetSubjectsByYearLevels(IEnumerable<CurriculumSubject> subjects, IEnumerable<YearLevelValue> yearLevelValues)
    {
        var filteredSubjects = new List<CurriculumSubject>();
        foreach (var subject in subjects)
        {
            var yearLevels = CurriculumSubjects.First(s => s.Id == subject.Id).YearLevels.Where(yl => yearLevelValues.Contains(yl.YearLevelValue));
            filteredSubjects.Add(CurriculumSubject.Create(subject.Id, subject.Name, yearLevels.ToList(), subject.Description));
        }

        return filteredSubjects;

    }

    public string GetSubjectName(SubjectId subjectId)
    {
        return CurriculumSubjects.FirstOrDefault(s => s.Id == subjectId)?.Name ?? string.Empty;
    }

    /// <summary>
    ///    Get the content descriptions for a given query and year levels.
    /// </summary>
    /// <param name="subjectId"></param>
    /// <param name="yearLevels"></param>
    /// <returns>A collection of lists of content descriptions, one for each YearLevelValue passed</returns>
    public Dictionary<YearLevelValue, List<ContentDescription>> GetContentDescriptions(SubjectId subjectId, List<YearLevelValue> yearLevels)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var query = context.CurriculumSubjects
            .Where(s => s.Id == subjectId)
            .Include(s => s.YearLevels)
            .ThenInclude(yl => yl.ConceptualOrganisers)
            .ThenInclude(co => co.ContentDescriptions);

        query.SelectMany(s => s.YearLevels).Where(yl => yearLevels.Contains(yl.YearLevelValue));
        var subject = query.First();

        var yearLevelContentDescriptions = new Dictionary<YearLevelValue, List<ContentDescription>>();
        foreach (var yl in yearLevels)
        {
            var yearLevel = subject.YearLevels.GetFromYearLevelValue(yl);
            var contentDescriptions = yearLevel.GetContentDescriptions();
            yearLevelContentDescriptions.Add(yl, contentDescriptions);
        }

        return yearLevelContentDescriptions;
    }
}