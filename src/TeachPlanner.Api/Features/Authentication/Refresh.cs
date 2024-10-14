using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TeachPlanner.Api.Domain.Users;
using TeachPlanner.Api.Interfaces.Authentication;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.Authentication;
using TeachPlanner.Shared.Exceptions;

namespace TeachPlanner.Api.Features.Authentication;

public static class Refresh
{
    public static async Task<IResult> Endpoint(RefreshModel request, ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new Command(request.AccessToken, request.RefreshToken);
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static ClaimsPrincipal? GetPrincipalFromExpiredToken(IConfiguration configuration, string token)
    {
        var secret = configuration["JwtSettings:Secret"] ??
                     throw new InvalidOperationException("Secret is not configured");

        var validation = new TokenValidationParameters
        {
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidAudience = configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = false
        };

        return new JwtSecurityTokenHandler().ValidateToken(token, validation, out var _);
    }

    public record Command(string AccessToken, string RefreshToken) : IRequest<AuthenticationResponse>;

    public sealed class Handler : IRequestHandler<Command, AuthenticationResponse>
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ITeacherRepository _teacherRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public Handler(IConfiguration configuration, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, ITeacherRepository teacherRepository)
        {
            _configuration = configuration;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _teacherRepository = teacherRepository;
        }

        public async Task<AuthenticationResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            var principal = GetPrincipalFromExpiredToken(_configuration, request.AccessToken);

            if (principal?.Identity?.Name == null)
            {
                throw new UnauthorizedAccessException();
            }

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException();
            }

            var teacher = await _teacherRepository.GetByUserId(user.Id, cancellationToken);
            if (teacher == null)
            {
                throw new TeacherNotFoundException();
            }

            var tokenResponse = _jwtTokenGenerator.GenerateToken(teacher, user.Email!);

            var refreshExpiryMinutes = _configuration["JWTSettings:RefreshTokenExpiryMinutes"] ??
                                       throw new InvalidOperationException("Secret not configured");
            user.RefreshToken = AuthenticationHelpers.GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(int.Parse(refreshExpiryMinutes));
            await _userManager.UpdateAsync(user);

            return new AuthenticationResponse(tokenResponse.Token, tokenResponse.Expiration,
                AuthenticationHelpers.GenerateRefreshToken(), teacher.AccountSetupComplete);
        }
    }
}