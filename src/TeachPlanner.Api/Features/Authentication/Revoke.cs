using Microsoft.AspNetCore.Identity;
using TeachPlanner.Shared.Domain.Users;

namespace TeachPlanner.Api.Features.Authentication;

public static class Revoke
{
    public static async Task<IResult> Delegate(HttpContext httpContext, UserManager<ApplicationUser> userManager, CancellationToken cancellationToken)
    {
        var username = httpContext.User.Identity?.Name;
        if (username is null)
        {
            throw new UnauthorizedAccessException();
        }

        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }

        user.RefreshToken = null;
        await userManager.UpdateAsync(user);

        return Results.Ok();
    }
}
