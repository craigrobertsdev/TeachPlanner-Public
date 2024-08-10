using FluentValidation;
using Mapster;
using MediatR;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Authentication;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.Authentication;
using Microsoft.AspNetCore.Identity;
using TeachPlanner.Shared.Domain.Users;

namespace TeachPlanner.Api.Features.Authentication;

public static class Login
{
    public static async Task<IResult> Delegate(LoginModel request, ISender sender,
           CancellationToken cancellationToken)
    {
        var command = request.Adapt<Command>();
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    private static string NormaliseEmail(string email)
    {
        return email.Trim().ToUpper();
    }

    public record Command(string Email, string Password) : IRequest<AuthenticationResponse>;

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    internal sealed class Handler : IRequestHandler<Command, AuthenticationResponse>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ITeacherRepository _teacherRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public Handler(IJwtTokenGenerator jwtTokenGenerator,
                       ITeacherRepository teacherRepository,
                       UserManager<ApplicationUser> userManager,
                       IConfiguration configuration)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _teacherRepository = teacherRepository;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthenticationResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(NormaliseEmail(request.Email));

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                throw new InvalidCredentialsException();
            }

            var teacher = await _teacherRepository.GetByUserId(user.Id, cancellationToken);
            if (teacher == null) throw new TeacherNotFoundException();

            var tokenResponse = _jwtTokenGenerator.GenerateToken(teacher, user.Email!);

            var refreshExpiryMinutes = _configuration["JWTSettings:RefreshTokenExpiryMinutes"] ?? throw new InvalidOperationException("Secret not configured");
            user.RefreshToken = AuthenticationHelpers.GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(int.Parse(refreshExpiryMinutes));
            await _userManager.UpdateAsync(user);

            return new AuthenticationResponse(tokenResponse.Token, tokenResponse.Expiration, user.RefreshToken, teacher.AccountSetupComplete);
        }
    }
}