using TeachPlanner.Api.Domain.Common.Interfaces;
using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Domain.Students;
using TeachPlanner.Api.Domain.WeekPlanners;
using TeachPlanner.Api.Domain.YearDataRecords.DomainEvents;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.YearDataRecords;

public class YearData : Entity<YearDataId>, IAggregateRoot
{
    private readonly List<LessonPlan> _lessonPlans = [];
    private readonly List<Student> _students = [];
    private readonly List<Subject> _subjects = [];
    private readonly List<WeekPlanner> _weekPlanners = [];
    private readonly List<YearLevelValue> _yearLevelsTaught = [];

    private YearData(YearDataId id, TeacherId teacherId, WeekStructure weekStructure, int calendarYear) : base(id)
    {
        TeacherId = teacherId;
        CalendarYear = calendarYear;
        WeekStructure = weekStructure;
    }

    private YearData(YearDataId id, TeacherId teacherId, WeekStructure weekStructure, int calendarYear,
        List<string> yearLevels) : base(id)
    {
        TeacherId = teacherId;
        CalendarYear = calendarYear;
        WeekStructure = weekStructure;
        var yearLevelEnums = yearLevels.Select(Enum.Parse<YearLevelValue>).ToList();
        yearLevelEnums.Sort();
        _yearLevelsTaught = yearLevelEnums;
    }

    public WeekStructure WeekStructure { get; private set; }
    public TeacherId TeacherId { get; private set; }
    public TermPlannerId? TermPlannerId { get; private set; }
    public int CalendarYear { get; private set; }
    public IReadOnlyList<Student> Students => _students.AsReadOnly();
    public IReadOnlyList<YearLevelValue> YearLevelsTaught => _yearLevelsTaught.AsReadOnly();
    public IReadOnlyList<Subject> Subjects => _subjects.AsReadOnly();
    public IReadOnlyList<LessonPlan> LessonPlans => _lessonPlans.AsReadOnly();
    public IReadOnlyList<WeekPlanner> WeekPlanners => _weekPlanners.AsReadOnly();

    public static YearData Create(TeacherId teacherId, int calendarYear, WeekStructure weekStructure)
    {
        var yearData = new YearData(new YearDataId(Guid.NewGuid()), teacherId, weekStructure, calendarYear);
        yearData.AddDomainEvent(new YearDataCreatedDomainEvent(Guid.NewGuid(), yearData.Id, calendarYear, teacherId));

        return yearData;
    }

    public static YearData Create(TeacherId teacherId, int calendarYear, WeekStructure weekStructure,
        List<string> yearLevels)
    {
        var yearData = new YearData(new YearDataId(Guid.NewGuid()), teacherId, weekStructure, calendarYear, yearLevels);
        yearData.AddDomainEvent(new YearDataCreatedDomainEvent(Guid.NewGuid(), yearData.Id, calendarYear, teacherId));

        return yearData;
    }

    public void AddSubjects(List<CurriculumSubject> subjects)
    {
        foreach (var subject in subjects)
        {
            if (IsInSubjects(subject))
            {
                return;
            }

            _subjects.Add(Subject.Create(subject.Id, subject.Name, []));
        }
    }

    private bool IsInSubjects(CurriculumSubject subject)
    {
        return _subjects.FirstOrDefault(s => s.Name == subject.Name) != null;
    }

    public void AddStudents(List<Student> students)
    {
        foreach (var student in students)
        {
            AddStudent(student);
        }
    }

    public void AddStudent(Student student)
    {
        if (NotInStudents(student))
        {
            _students.Add(student);
        }
    }

    private bool NotInStudents(Student student)
    {
        return !_students.Contains(student);
    }

    public void AddYearLevel(YearLevelValue yearLevel)
    {
        if (NotInYearLevelsTaught(yearLevel))
        {
            _yearLevelsTaught.Add(yearLevel);
        }
    }

    private bool NotInYearLevelsTaught(YearLevelValue yearLevel)
    {
        return _yearLevelsTaught.Contains(yearLevel);
    }

    public void AddTermPlanner(TermPlannerId termPlannerId)
    {
        if (TermPlannerId is not null)
        {
            throw new TermPlannerAlreadyAssociatedException();
        }

        TermPlannerId = termPlannerId;
    }

    public void AddYearLevelsTaught(List<YearLevelValue> yearLevelsTaught)
    {
        foreach (var yearLevel in yearLevelsTaught)
        {
            AddYearLevel(yearLevel);
        }
    }

    public void SetWeekStructure(WeekStructure weekStructure)
    {
        WeekStructure = weekStructure;
        _domainEvents.Add(new WeekStructureAddedToYearDataEvent(Guid.NewGuid(), WeekStructure.Id));
    }

    public void UpdateWeekStructure(WeekStructure weekStructure)
    {
        WeekStructure.SetPeriods(weekStructure.Periods);
        WeekStructure.SetDayTemplates(weekStructure.DayTemplates);
    }

    public void SetYearLevelsTaught(List<YearLevelValue> yearLevels)
    {
        yearLevels.Sort();
        _yearLevelsTaught.Clear();
        _yearLevelsTaught.AddRange(yearLevels);
    }

    public void AddWeekPlanner(WeekPlanner weekPlanner)
    {
        _weekPlanners.Add(weekPlanner);
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private YearData()
    {
    }
}