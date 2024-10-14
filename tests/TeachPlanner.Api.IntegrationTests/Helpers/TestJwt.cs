using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TeachPlanner.Api.IntegrationTests.Helpers;

public class TestJwt
{
        public List<Claim> Claims { get; } = [];
        public int ExpiresInMinutes { get; set; } = 30;

        public TestJwt WithId(Guid id)
        {
            Claims.Add(new Claim("id", id.ToString()));
            return this;
        }

        public TestJwt WithUserId(string userId)
        {
            Claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userId));
            return this;
        }

        public TestJwt WithFirstName(string firstName)
        {
            Claims.Add(new Claim(JwtRegisteredClaimNames.GivenName, firstName));
            return this;
        }

        public TestJwt WithLastName(string lastName)
        {
            Claims.Add(new Claim(JwtRegisteredClaimNames.FamilyName, lastName));
            return this;
        }

        public TestJwt WithEmail(string email)
        {
            Claims.Add(new Claim(ClaimTypes.Email, email));
            return this;
        }

        public string Build()
        {
            var token = new JwtSecurityToken(
                JwtProvider.Issuer,
                JwtProvider.Issuer,
                Claims,
                expires: DateTime.Now.AddMinutes(ExpiresInMinutes),
                signingCredentials: JwtProvider.SigningCredentials
            );
            return JwtProvider.JwtSecurityTokenHandler.WriteToken(token);

        }
}