using MediatR;
using TeachPlanner.Api.Domain.PlannerTemplates;
using TeachPlanner.Api.Extensions;
using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Api.Interfaces.Services;
using TeachPlanner.Shared.Contracts.WeekPlanners;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.WeekPlanners;

public static class GetWeekPlanner
{
    public static async Task<IResult> Endpoint(Guid teacherId, int week, int term, int year, ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new Query(new TeacherId(teacherId), week, term, year);

        var result = await sender.Send(query, cancellationToken);

        if (result is null)
        {
            return Results.NotFound();
        }
        
        return Results.Ok(result);
    }

    public record Query(TeacherId TeacherId, int WeekNumber, int TermNumber, int Year) : IRequest<WeekPlannerDto?>;

    public sealed class Handler(
        ITeacherRepository teacherRepository,
        IWeekPlannerRepository weekPlannerRepository,
        IYearDataRepository yearDataRepository,
        ITermDatesService termDatesService,
        IUnitOfWork unitOfWork,
        IPlannerTemplateRepository plannerTemplateRepository,
        ICurriculumService curriculumService) : IRequestHandler<Query, WeekPlannerDto?>
    {
        public async Task<WeekPlannerDto?> Handle(Query request, CancellationToken cancellationToken)
        {
        var teacher = await teacherRepository.GetById(request.TeacherId, cancellationToken)
                          ?? throw new TeacherNotFoundException();
            var yearData =
                await yearDataRepository.GetByTeacherIdAndYear(request.TeacherId, request.Year, cancellationToken)
                ?? throw new YearDataNotFoundException();
            var weekPlanner = await weekPlannerRepository.GetWeekPlanner(yearData.Id, request.WeekNumber,
                request.TermNumber, request.Year, cancellationToken);
            var weekStructure = await plannerTemplateRepository.GetByTeacherId(teacher.Id, cancellationToken);

            if (weekPlanner is null)
            {
                return null;
            }

            var resources = await teacherRepository.GetResourcesById(
                weekPlanner.DayPlans.SelectMany(dp =>
                    dp.LessonPlans.SelectMany(lp => lp.Resources.Select(r => r.Id))), cancellationToken);
            var dayPlanDtos = weekPlanner.DayPlans.ToDtos(resources, curriculumService.CurriculumSubjects);

            return new WeekPlannerDto(
                dayPlanDtos,
                weekStructure!.ToDto(),
                weekPlanner.WeekStart,
                weekPlanner.WeekNumber
            );
        }
    }
}