using System.Security.Cryptography;

namespace TeachPlanner.Api.Features.Authentication;

public static class AuthenticationHelpers
{
    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
