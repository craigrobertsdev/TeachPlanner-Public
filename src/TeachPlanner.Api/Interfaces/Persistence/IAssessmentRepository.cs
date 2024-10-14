using TeachPlanner.Api.Domain.Assessments;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Interfaces.Persistence;

public interface IAssessmentRepository : IRepository<Assessment>
{
    public Task<List<Assessment>> GetAssessmentsById(List<AssessmentId> assessmentIds,
        CancellationToken cancellationToken);
}