using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.Users;
using TeachPlanner.Api.Interfaces.Authentication;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.Authentication;
using TeachPlanner.Shared.Exceptions;

namespace TeachPlanner.Api.Features.Authentication;

public static class Register
{
    public static async Task<IResult> Endpoint(RegisterModel request, ISender sender,
        CancellationToken cancellationToken)
    {
        var command = request.Adapt<Command>();
        var result = await sender.Send(command, cancellationToken);
        return Results.Ok(result);
    }

    public record Command(string FirstName, string LastName, string Email, string Password, string ConfirmedPassword)
        : IRequest<AuthenticationResponse>;


    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.FirstName).NotEmpty();
            RuleFor(c => c.LastName).NotEmpty();
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
            RuleFor(c => c.Password).NotEmpty().MinimumLength(10);
        }
    }

    internal sealed class Handler : IRequestHandler<Command, AuthenticationResponse>
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public Handler(IJwtTokenGenerator jwtTokenGenerator, ITeacherRepository teacherRepository,
            IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthenticationResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.Password != request.ConfirmedPassword)
            {
                throw new PasswordsDoNotMatchException();
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                throw new DuplicateEmailException();
            }

            var refreshExpiryMinutes = _configuration["JWTSettings:RefreshTokenExpiryMinutes"] ??
                                       throw new InvalidOperationException("Secret not configured");

            user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                RefreshToken = AuthenticationHelpers.GenerateRefreshToken(),
                RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(int.Parse(refreshExpiryMinutes))
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new UserRegistrationFailedException(string.Join(" ", result.Errors.Select(e => e.Description)));
            }

            var teacher = Teacher.Create(user.Id, request.FirstName, request.LastName);
            _teacherRepository.Add(teacher);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var tokenResponse = _jwtTokenGenerator.GenerateToken(teacher, user.Email);

            return new AuthenticationResponse(request.FirstName, request.LastName, tokenResponse.Token, tokenResponse.Expiration, user.RefreshToken, false);
        }
    }
}