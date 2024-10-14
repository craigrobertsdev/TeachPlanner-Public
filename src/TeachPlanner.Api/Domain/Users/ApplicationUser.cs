using Microsoft.AspNetCore.Identity;

namespace TeachPlanner.Api.Domain.Users;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}