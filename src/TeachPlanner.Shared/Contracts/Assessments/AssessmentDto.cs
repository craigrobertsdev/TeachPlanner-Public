using TeachPlanner.Shared.Domain.Assessments;
using TeachPlanner.Shared.Domain.Common.Enums;

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
