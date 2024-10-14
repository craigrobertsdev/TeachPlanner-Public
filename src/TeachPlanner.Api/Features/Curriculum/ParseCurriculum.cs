using MediatR;
using TeachPlanner.Api.Interfaces.Curriculum;
using TeachPlanner.Api.Interfaces.Persistence;

namespace TeachPlanner.Api.Features.Curriculum;

public static class ParseCurriculum
{
    public static async Task<IResult> Endpoint(ISender sender, CancellationToken cancellationToken)
    {
        var command = new Command();
        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }

    public record Command : IRequest;

    internal sealed class Handler : IRequestHandler<Command>
    {
        private readonly ICurriculumParser _curriculumParser;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly IUnitOfWork _unitOfWork;

        public Handler(ICurriculumParser curriculumParser, IUnitOfWork unitOfWork,
            ICurriculumRepository curriculumRepository)
        {
            _curriculumParser = curriculumParser;
            _unitOfWork = unitOfWork;
            _curriculumRepository = curriculumRepository;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var subjects = await _curriculumParser.ParseCurriculum();
            await _curriculumRepository.AddCurriculum(subjects, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}