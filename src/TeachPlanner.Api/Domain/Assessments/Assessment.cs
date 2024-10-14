using TeachPlanner.Api.Domain.Common.Interfaces;
using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.Assessments;

public class Assessment : Entity<AssessmentId>, IAggregateRoot
{
    protected Assessment(
        AssessmentId id,
        TeacherId teacherId,
        SubjectId subjectId,
        StudentId studentId,
        YearLevelValue yearLevel,
        string planningNotes,
        DateTime conductedDateTime) : base(id)
    {
        Id = id;
        TeacherId = teacherId;
        SubjectId = subjectId;
        StudentId = studentId;
        YearLevel = yearLevel;
        PlanningNotes = planningNotes;
        DateConducted = conductedDateTime;
    }

    public TeacherId TeacherId { get; private set; }
    public SubjectId SubjectId { get; private set; }
    public StudentId StudentId { get; private set; }
    public YearLevelValue YearLevel { get; private set; }
    public AssessmentType AssessmentType { get; }
    public AssessmentResult? AssessmentResult { get; private set; }
    public string PlanningNotes { get; private set; }
    public DateTime DateConducted { get; private set; }
    public DateTime CreatedDateTime { get; }
    public DateTime UpdatedDateTime { get; private set; }

    public void SetAssessmentResult(AssessmentResult result)
    {
        if (AssessmentResult is not null)
        {
            AssessmentResult = result;
            UpdatedDateTime = DateTime.Now;
        }
    }

    public Assessment Create(
        TeacherId teacherId,
        SubjectId subjectId,
        StudentId studentId,
        YearLevelValue yearLevel,
        string planningNotes,
        DateTime conductedDateTime)
    {
        return new Assessment(
            new AssessmentId(Guid.NewGuid()),
            teacherId,
            subjectId,
            studentId,
            yearLevel,
            planningNotes,
            conductedDateTime);
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Assessment()
    {
    }
}