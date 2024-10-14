using TeachPlanner.Api.Domain.Assessments;
using TeachPlanner.Api.Domain.Common.Interfaces;
using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.Teachers.DomainEvents;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.Teachers;

public sealed class Teacher : Entity<TeacherId>, IAggregateRoot
{
    private readonly List<Assessment> _assessments = [];
    private readonly List<Resource> _resources = [];
    private readonly List<SubjectId> _subjectsTaught = [];
    private readonly List<YearDataEntry> _yearDataHistory = [];

    private Teacher(TeacherId id, string userId, string firstName, string lastName) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        UserId = userId;
    }

    public string UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool AccountSetupComplete { get; private set; }
    public int LastSelectedYear { get; private set; }
    public DateOnly LastSelectedWeekStart { get; private set; }
    public IReadOnlyList<Assessment> Assessments => _assessments.AsReadOnly();
    public IReadOnlyList<Resource> Resources => _resources.AsReadOnly();
    public IReadOnlyList<YearDataEntry> YearDataHistory => _yearDataHistory.AsReadOnly();
    public IReadOnlyList<SubjectId> SubjectsTaught => _subjectsTaught.AsReadOnly();

    public static Teacher Create(string userId, string firstName, string lastName)
    {
        var teacher = new Teacher(new TeacherId(Guid.NewGuid()), userId, firstName, lastName);
        teacher.AddDomainEvent(new TeacherCreatedDomainEvent(Guid.NewGuid(), teacher.Id));

        return teacher;
    }

    public YearDataId? GetYearData(int year)
    {
        var yearDataEntry = _yearDataHistory.FirstOrDefault(yd => yd.CalendarYear == year);
        if (yearDataEntry is null)
        {
            return null;
        }

        return yearDataEntry.YearDataId;
    }

    public void AddYearData(YearDataEntry yearDataEntry)
    {
        if (!YearDataExists(yearDataEntry))
        {
            _yearDataHistory.Add(YearDataEntry.Create(yearDataEntry.CalendarYear, yearDataEntry.YearDataId));
        }
    }

    private bool YearDataExists(YearDataEntry yearDataEntry)
    {
        return YearDataExists(yearDataEntry.CalendarYear);
    }

    private bool YearDataExists(int year)
    {
        return _yearDataHistory.FirstOrDefault(yd => yd.CalendarYear == year) is not null;
    }

    public void SetSubjectsTaught(List<SubjectId> subjects)
    {
        foreach (var subject in _subjectsTaught.ToList())
        {
            if (!subjects.Contains(subject))
            {
                _subjectsTaught.Remove(subject);
            }
        }

        foreach (var subject in subjects)
        {
            if (_subjectsTaught.Contains(subject))
            {
                continue;
            }

            _subjectsTaught.Add(subject);
        }
    }

    public void CompleteAccountSetup(int lastSelectedYear, DateOnly lastSelectedWeekStart)
    {
        LastSelectedYear = lastSelectedYear;
        LastSelectedWeekStart = lastSelectedWeekStart;
        AccountSetupComplete = true;
    }

    public void SetLastSelectedYear(int year)
    {
        LastSelectedYear = year;
    }
    
    public void SetLastSelectedWeekStart(DateOnly weekStart)
    {
        LastSelectedWeekStart = weekStart;
    }

    public void AddResource(Resource resource)
    {
        if (!_resources.Contains(resource))
        {
            _resources.Add(resource);
        }
    }

    public void AddResources(IEnumerable<Resource> resources)
    {
        foreach (var resource in resources)
        {
            AddResource(resource);
        }
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Teacher()
    {
    }
}