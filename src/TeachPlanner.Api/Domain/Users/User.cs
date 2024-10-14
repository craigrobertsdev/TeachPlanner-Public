using System.Security.Claims;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.Users;

public sealed class User
{
    public User(string email, string password)
    {
        Id = new UserId(Guid.NewGuid());
        Email = email;
        Password = password;
    }

    public UserId Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public ClaimsPrincipal ToClaimsPrincipal => new(
        new ClaimsIdentity(
            [new(ClaimTypes.NameIdentifier, Id.Value.ToString()), new(ClaimTypes.Name, Email)],
            "TeachPlanner"));

    public static User FromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
    {
        return new User
        {
            Id = new UserId(Guid.Parse(claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value)),
            Email = claimsPrincipal.FindFirst(ClaimTypes.Name)!.Value
        };
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private User()
    {
    }
}