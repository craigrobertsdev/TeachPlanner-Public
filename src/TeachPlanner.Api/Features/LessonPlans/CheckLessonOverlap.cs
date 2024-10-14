using TeachPlanner.Api.Interfaces.Persistence;
using TeachPlanner.Shared.Exceptions;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Features.LessonPlans;

public static class CheckLessonOverlap
{
    public static async Task<bool> Endpoint(
        Guid teacherId,
        Guid? lessonPlanId,
        DateOnly lessonDate,
        int lessonNumber,
        int numberOfPeriods,
        IYearDataRepository yearDataRepository,
        ILessonPlanRepository lessonPlanRepository,
        CancellationToken cancellationToken)
    {
        if (lessonNumber < 1 || numberOfPeriods < 1)
        {
            throw new ArgumentOutOfRangeException("Lesson number and number of periods must not be less than 0");
        }

        var yearData =
            await yearDataRepository.GetByTeacherIdAndYear(new TeacherId(teacherId), lessonDate.Year,
                cancellationToken);
        if (yearData == null)
        {
            throw new YearDataNotFoundException();
        }

        var lessonPlans = await lessonPlanRepository.GetByDate(yearData.Id, lessonDate, cancellationToken);
        if (lessonPlans.Count == 0)
        {
            return false;
        }

        if (lessonPlanId is not null &&
            lessonPlans.Any(lp => lp.Id.Value == lessonPlanId && lp.NumberOfLessons == numberOfPeriods))
        {
            return false;
        }

        var lp = lessonPlans.FirstOrDefault(lp => lp.StartPeriod == lessonNumber);

        if (lp is not null && lp?.Id.Value != lessonPlanId)
        {
            return true;
        }

        if (lessonPlans.Where(lp => lp.StartPeriod > lessonNumber && lp.StartPeriod < lessonNumber + numberOfPeriods)
            .Any())
        {
            return true;
        }

        return false;
    }
}