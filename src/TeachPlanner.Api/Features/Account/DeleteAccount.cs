using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.Users;

namespace TeachPlanner.Api.Features.Account;

public static class DeleteAccount
{
    public static async Task<IResult> Delegate([FromRoute] Guid teacherId, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new Command(new TeacherId(teacherId)), cancellationToken);
        return Results.Ok();
    }

    public record Command(TeacherId TeacherId) : IRequest;

    public sealed class Handler : IRequestHandler<Command>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
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
            var user = await _userManager.FindByIdAsync(teacher.UserId.ToString());
            await _userManager.DeleteAsync(user!);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
