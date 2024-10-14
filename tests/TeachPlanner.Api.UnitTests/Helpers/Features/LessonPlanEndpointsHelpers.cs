using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Features.LessonPlans;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Helpers.Features;

public static class LessonPlanEndpointHelpers
{
    public static CreateLessonPlan.Command CreateValidCreateLessonPlanCommand(Teacher teacher, SubjectId subjectId,
        int calendarYear, int numberOfPeriods, int lessonNumber)
    {
        return new CreateLessonPlan.Command(teacher.Id, subjectId, [], "", "", new DateOnly(calendarYear, 1, 1),
            numberOfPeriods, lessonNumber, []);
    }
}