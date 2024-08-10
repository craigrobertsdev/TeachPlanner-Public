using TeachPlanner.Shared.Domain.Teachers.DomainEvents;
using TeachPlanner.Shared.Domain.Assessments;
using TeachPlanner.Shared.Domain.Common.Interfaces;
using TeachPlanner.Shared.Domain.Common.Primatives;
using TeachPlanner.Shared.Domain.Curriculum;
using TeachPlanner.Shared.Domain.YearDataRecords;

namespace TeachPlanner.Shared.Domain.Teachers;

public sealed class Teacher : Entity<TeacherId>, IAggregateRoot
{
    private readonly List<Assessment> _assessments = new();
    private readonly List<Resource> _resources = new();
    private readonly List<YearDataEntry> _yearDataHistory = new();
    private readonly List<CurriculumSubject> _subjectsTaught = new();

    public string UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool AccountSetupComplete { get; private set; }
    public IReadOnlyList<Assessment> Assessments => _assessments.AsReadOnly();
    public IReadOnlyList<Resource> Resources => _resources.AsReadOnly();
    public IReadOnlyList<YearDataEntry> YearDataHistory => _yearDataHistory.AsReadOnly();
    public IReadOnlyList<CurriculumSubject> SubjectsTaught => _subjectsTaught.AsReadOnly();

    private Teacher(TeacherId id, string userId, string firstName, string lastName) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        UserId = userId;
    }

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
            _yearDataHistory.Add(YearDataEntry.Create(yearDataEntry.CalendarYear, yearDataEntry.YearDataId));
    }

    private bool YearDataExists(YearDataEntry yearDataEntry)
    {
        return YearDataExists(yearDataEntry.CalendarYear);
    }

    private bool YearDataExists(int year)
    {
        return _yearDataHistory.FirstOrDefault(yd => yd.CalendarYear == year) is not null;
    }

    public void SetSubjectsTaught(List<CurriculumSubject> subjects)
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

    public void CompleteAccountSetup()
    {
        AccountSetupComplete = true;
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