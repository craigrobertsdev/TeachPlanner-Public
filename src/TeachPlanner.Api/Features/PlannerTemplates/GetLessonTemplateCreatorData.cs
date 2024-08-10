using TeachPlanner.Shared.Common.Exceptions;
using TeachPlanner.Shared.Common.Interfaces.Persistence;
using TeachPlanner.Shared.Common.Interfaces.Services;
using TeachPlanner.Shared.Contracts.PlannerTemplates;
using TeachPlanner.Shared.Domain.PlannerTemplates;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Api.Features.PlannerTemplates;

public static class GetLessonTemplateCreatorData
{
    public static async Task<IResult> Delegate(
        Guid teacherId,
        int calendarYear,
        ITeacherRepository teacherRepository,
        IYearDataRepository yearDataRepository,
        ICurriculumService curriculumService,
        CancellationToken cancellationToken)
    {
        var teacher = await teacherRepository.GetById(new TeacherId(teacherId), cancellationToken)
            ?? throw new TeacherNotFoundException();

        var yearDataId = teacher.GetYearData(calendarYear != 0 ? calendarYear : DateTime.Now.Year)
            ?? throw new YearDataNotFoundException();

        var yearData = await yearDataRepository.GetById(yearDataId, cancellationToken)
            ?? throw new YearDataNotFoundException();
        var subjectNames = curriculumService.GetSubjectNames();

        return Results.Ok(new LessonTemplateCreatorDto(
            yearData.WeekStructure.ToDto(),
            subjectNames));
    }
}
