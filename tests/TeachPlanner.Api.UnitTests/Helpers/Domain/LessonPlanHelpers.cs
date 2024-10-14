using TeachPlanner.Api.Domain.LessonPlans;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Tests.Helpers.Domain;

internal static class LessonPlanHelpers
{
    public static LessonPlan CreateLessonPlan()
    {
        return LessonPlan.Create(
            new YearDataId(Guid.NewGuid()),
            new SubjectId(Guid.NewGuid()),
            [Guid.NewGuid(), Guid.NewGuid()],
            "This is going to be a great lesson!.",
            "<p>This is going to be a great lesson!.</p>",
            1,
            1,
            new DateOnly(2024, 1, 29),
            []);
    }

    public static LessonPlan CreateLessonPlan(SubjectId subjectId)
    {
        return LessonPlan.Create(
            new YearDataId(Guid.NewGuid()),
            subjectId,
            [Guid.NewGuid(), Guid.NewGuid()],
            "This is going to be a great lesson!.",
            "<p>This is going to be a great lesson!.</p>",
            1,
            1,
            new DateOnly(2024, 1, 29),
            []);
    }

    public static LessonPlan CreateLessonPlan(SubjectId subjectId, List<Resource> resources)
    {
        return LessonPlan.Create(
            new YearDataId(Guid.NewGuid()),
            subjectId,
            [Guid.NewGuid(), Guid.NewGuid()],
            "This is going to be a great lesson!.",
            "<p>This is going to be a great lesson!.</p>",
            1,
            1,
            new DateOnly(2024, 1, 29),
            resources);
    }

    public static LessonPlan CreateLessonPlan(int startPeriod, int numberOfPeriods)
    {
        return LessonPlan.Create(
            new YearDataId(Guid.NewGuid()),
            new SubjectId(Guid.NewGuid()),
            [Guid.NewGuid(), Guid.NewGuid()],
            "This is going to be a great lesson!.",
            "<p>This is going to be a great lesson!.</p>",
            numberOfPeriods,
            startPeriod,
            new DateOnly(2024, 1, 29),
            []);
    }
}