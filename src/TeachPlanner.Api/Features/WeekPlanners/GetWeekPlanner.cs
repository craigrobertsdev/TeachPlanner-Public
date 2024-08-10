using MediatR;
using TeachPlanner.Api.Extensions;
using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Common.Interfaces.Services;
using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.WeekPlanners;

namespace TeachPlanner.Api.Features.WeekPlanners;

public static class GetWeekPlanner
{
    public record Query(TeacherId TeacherId, int WeekNumber, int TermNumber, int Year) : IRequest<WeekPlannerDto>;

    public sealed class Handler(
        ITeacherRepository teacherRepository,
        IWeekPlannerRepository weekPlannerRepository,
        IYearDataRepository yearDataRepository,
        ITermDatesService termDatesService,
        IUnitOfWork unitOfWork,
        IPlannerTemplateRepository plannerTemplateRepository,
        ICurriculumService curriculumService) : IRequestHandler<Query, WeekPlannerDto>
    {
        private readonly ITeacherRepository _teacherRepository = teacherRepository;
        private readonly IWeekPlannerRepository _weekPlannerRepository = weekPlannerRepository;
        private readonly IYearDataRepository _yearDataRepository = yearDataRepository;
        private readonly ITermDatesService _termDatesService = termDatesService;
        private readonly IPlannerTemplateRepository _plannerTemplateRepository = plannerTemplateRepository;
        private readonly ICurriculumService _curriculumService = curriculumService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<WeekPlannerDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var teacher = await _teacherRepository.GetById(request.TeacherId, cancellationToken) ?? throw new TeacherNotFoundException();
            var yearData = await _yearDataRepository.GetByTeacherIdAndYear(request.TeacherId, request.Year, cancellationToken);
            var weekPlanner = await _weekPlannerRepository.GetWeekPlanner(yearData.Id, request.WeekNumber, request.TermNumber, request.Year, cancellationToken);
            var weekStructure = await _plannerTemplateRepository.GetByTeacherId(teacher.Id, cancellationToken);

            if (weekPlanner is not null)
            {
                var resources = await _teacherRepository.GetResourcesById(weekPlanner.DayPlans.SelectMany(dp => dp.LessonPlans.SelectMany(lp => lp.Resources.Select(r => r.Id))), cancellationToken);
                var dayPlanDtos = weekPlanner.DayPlans.ToDtos(resources, _curriculumService.CurriculumSubjects);

                return new WeekPlannerDto(
                    dayPlanDtos,
                    DayPlanTemplateDto.Create(weekStructure!),
                    weekPlanner.WeekStart,
                    weekPlanner.WeekNumber
                );
            }


            weekPlanner = WeekPlanner.Create(
                yearData!.Id,
                1,
                request.WeekNumber,
                request.Year,
                _termDatesService.GetWeekStart(request.Year, request.TermNumber, request.WeekNumber));

            _weekPlannerRepository.Add(weekPlanner);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new WeekPlannerDto(
                new List<DayPlanDto>(),
                DayPlanTemplateDto.Create(weekStructure),
                weekPlanner.WeekStart,
                request.WeekNumber); ;
        }
    }

    public static async Task<IResult> Delegate(Guid teacherId, int week, int term, int year, ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new Query(new TeacherId(teacherId), week, term, year);

        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }
}