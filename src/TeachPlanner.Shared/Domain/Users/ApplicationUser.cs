using Microsoft.AspNetCore.Identity;

namespace TeachPlanner.Shared.Domain.Users;
public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}

