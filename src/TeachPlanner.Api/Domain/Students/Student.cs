using TeachPlanner.Api.Domain.Assessments;
using TeachPlanner.Api.Domain.Common.Interfaces;
using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Api.Domain.Reports;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.Students;

public sealed class Student : Entity<StudentId>, IAggregateRoot
{
    private readonly List<Assessment> _assessments = [];
    private readonly List<Report> _reports = [];

    private Student(
        StudentId id,
        TeacherId teacherId,
        string firstName,
        string lastName) : base(id)
    {
        TeacherId = teacherId;
        FirstName = firstName;
        LastName = lastName;
    }

    public TeacherId TeacherId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }


    public IReadOnlyList<Report> Reports => _reports;
    public IReadOnlyList<Assessment> Assessments => _assessments;

    public static Student Create(TeacherId teacherId, string firstName, string lastName)
    {
        return new Student(
            new StudentId(Guid.NewGuid()),
            teacherId,
            firstName,
            lastName);
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Student()
    {
    }
}