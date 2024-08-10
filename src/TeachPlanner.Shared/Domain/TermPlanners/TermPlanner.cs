using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Domain.TermPlanners.DomainEvents;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Common.Interfaces;
using TeachPlanner.Shared.Domain.Common.Primatives;
using TeachPlanner.Shared.Domain.YearDataRecords;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Shared.Domain.TermPlanners;

public sealed class TermPlanner : Entity<TermPlannerId>, IAggregateRoot
{
    private readonly List<TermPlan> _termPlans = new();
    private readonly List<YearLevelValue> _yearLevels = new();

    public IReadOnlyList<TermPlan> TermPlans => _termPlans.AsReadOnly();
    public IReadOnlyList<YearLevelValue> YearLevels => _yearLevels.AsReadOnly();
    public YearDataId YearDataId { get; private set; }
    public int CalendarYear { get; private set; }

    private TermPlanner(TermPlannerId id, YearDataId yearDataId, int calendarYear,
        List<YearLevelValue> yearLevels) : base(id)
    {
        YearDataId = yearDataId;
        CalendarYear = calendarYear;
        _yearLevels = yearLevels;

        SortYearLevels();
    }

    public static TermPlanner Create(YearDataId yearDataId, int calendarYear, List<YearLevelValue> yearLevels)
    {
        yearLevels = RemoveDuplicateYearLevels(yearLevels);

        var termPlanner = new TermPlanner(
            new TermPlannerId(Guid.NewGuid()),
            yearDataId,
            calendarYear,
            yearLevels);

        termPlanner.AddDomainEvent(new TermPlannerCreatedDomainEvent(Guid.NewGuid(), termPlanner.Id, yearDataId));
        return termPlanner;
    }

    private static List<YearLevelValue> RemoveDuplicateYearLevels(List<YearLevelValue> yearLevels)
    {
        return yearLevels.Distinct().ToList();
    }

    public void AddYearLevel(YearLevelValue yearLevel)
    {
        if (_yearLevels.Contains(yearLevel)) throw new InputException("Year level already exists");

        _yearLevels.Add(yearLevel);
        SortYearLevels();
    }

    public void SortYearLevels()
    {
        if (_yearLevels.Count == 1) return;

        _yearLevels.Sort();
    }

    public void AddTermPlan(TermPlan termPlan)
    {
        if (_termPlans.Contains(termPlan)) throw new DuplicateTermPlanException();

        if (_termPlans.Count >= 4) throw new TooManyTermPlansException();

        if (_termPlans.Any(tp => tp.TermNumber == termPlan.TermNumber)) throw new DuplicateTermNumberException();

        _termPlans.Add(termPlan);
    }

    public void PopulateSubjectsForTerms(List<CurriculumSubject> subjects)
    {
        var subjectNumbersForTerms = TermPlans.Select(tp => tp.Subjects.Count)
            .ToArray();

        var subjectCounts = new[] { 0, 0, 0, 0 };

        for (var i = 0; i < subjectNumbersForTerms.Length; i++)
            for (var j = 0; j < subjectNumbersForTerms[i]; j++)
            {
                if (subjectCounts[i] >= subjectNumbersForTerms[i]) break;

                var subject = subjects.First(s => s.Id == _termPlans[i].Subjects[j].Id);

                if (subject is null) continue;

                _termPlans[i].SetSubjectAtIndex(subject, j);
                subjectCounts[i]++;
            }
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private TermPlanner()
    {
    }
}