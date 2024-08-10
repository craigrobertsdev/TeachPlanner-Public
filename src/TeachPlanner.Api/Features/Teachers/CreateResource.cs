using MediatR;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Contracts.Resources;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Api.Features.Teachers;

public static class CreateResource
{
    public static async Task<IResult> Delegate(ISender sender, Guid teacherId, CreateResourceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new Command(new TeacherId(teacherId), request.Name, new SubjectId(request.SubjectId),
            request.YearLevels, request.AssociatedStrands, request.IsAssessment);

        var result = await sender.Send(command, cancellationToken);

        return Results.Ok(result);
    }

    public record Command(TeacherId TeacherId, string Name, SubjectId SubjectId, List<YearLevelValue> YearLevels, List<string> AssociatedStrands,
        bool IsAssessment) : IRequest<string>;

    public sealed class Handler : IRequestHandler<Command, string>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(ITeacherRepository teacherRepository, IUnitOfWork unitOfWork)
        {
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            // TODO: Add method to upload files to storage and generate URL
            var url = "https://www.placeholder.com";

            var teacher = await _teacherRepository.GetById(request.TeacherId, cancellationToken);

            if (teacher is null) throw new TeacherNotFoundException();

            var resource = Resource.Create(
                request.TeacherId,
                request.Name,
                url,
                request.IsAssessment,
                request.SubjectId,
                request.YearLevels,
                request.AssociatedStrands);

            teacher.AddResource(resource);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return url;
        }
    }
}