using TeachPlanner.Shared.Contracts.LessonPlans;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public record LessonComment(
    string Content,
    bool Completed,
    bool StruckOut,
    DateTime? CompletedDateTime);

public static class LessonCommentModelExtensions
{
    public static IEnumerable<LessonComment> ConvertFromDtos(this IEnumerable<LessonCommentDto> lessonComments)
    {
        return lessonComments.Select(lc => new LessonComment(
            lc.Content,
            lc.Completed,
            lc.StruckOut,
            lc.CompletedDateTime));
    }
}