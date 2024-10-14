using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Api.Domain.Users;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.Account;

public static class DeleteAccount
{
    public static async Task<IResult> Endpoint([FromRoute] Guid teacherId, ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new Command(new TeacherId(teacherId)), cancellationToken);
        }
        catch (TeacherNotFoundException)
        {
            return Results.NotFound();
        }

        return Results.Ok();
    }

    public record Command(TeacherId TeacherId) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public Handler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetById(request.TeacherId, cancellationToken);
            if (teacher == null)
            {
                throw new TeacherNotFoundException();
            }

            _teacherRepository.Delete(teacher);
            var user = await _userManager.FindByIdAsync(teacher.UserId);
            await _userManager.DeleteAsync(user!);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}