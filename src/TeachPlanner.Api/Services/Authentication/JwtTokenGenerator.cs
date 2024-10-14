using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Interfaces.Authentication;
using TeachPlanner.Shared.Models.Authentication;

namespace TeachPlanner.Api.Services.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public TokenResponse GenerateToken(Teacher teacher, string email)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email), 
            new Claim(JwtRegisteredClaimNames.Sub, teacher.UserId),
            new Claim(JwtRegisteredClaimNames.GivenName, teacher.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, teacher.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("id", teacher.Id.Value.ToString())
        };

        var securityToken = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return new TokenResponse(token, securityToken.ValidTo);
    }
}