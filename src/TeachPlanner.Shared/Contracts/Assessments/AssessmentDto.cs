using TeachPlanner.Shared.Enums;
using TeachPlanner.Shared.ValueObjects;

namespace TeachPlanner.Shared.Contracts.Assessments;

public record AssessmentDto(
    Guid SubjectId,
    Guid StudentId,
    YearLevelValue YearLevel,
    AssessmentResultResponse AssessmentResult,
    string PlanningNotes,
    DateTime ConductedDateTime);

public record AssessmentResultResponse(
    string Comments,
    AssessmentGrade Grade,
    DateTime DateMarked);