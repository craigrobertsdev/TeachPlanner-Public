using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace TeachPlanner.Api.IntegrationTests.Helpers;

public class JwtProvider
{
    // Our fake issuer 
    public static string Issuer { get; } = "Test_Auth_Server";

    // Our random signing key - used to sign and validate the tokens
    public static SecurityKey SecurityKey { get; } = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());

    // the signing credentials used by the token handler to sign tokens
    public static SigningCredentials SigningCredentials { get; } =
        new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

    // the token handler we'll use to actually issue tokens
    public static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();
    
}