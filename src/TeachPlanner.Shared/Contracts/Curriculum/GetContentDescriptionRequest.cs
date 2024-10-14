namespace TeachPlanner.Shared.Contracts.Curriculum;

public record GetContentDescriptionRequest(Guid SubjectId, List<string> YearLevels);