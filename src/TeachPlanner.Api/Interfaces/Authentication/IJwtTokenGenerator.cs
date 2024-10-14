using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Shared.Models.Authentication;

namespace TeachPlanner.Api.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    TokenResponse GenerateToken(Teacher teacher, string email);
}