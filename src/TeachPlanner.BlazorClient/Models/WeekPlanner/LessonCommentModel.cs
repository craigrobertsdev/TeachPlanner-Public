using TeachPlanner.Shared.Contracts.LessonPlans;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public record LessonCommentModel(
    string Content,
    bool Completed,
    bool StruckOut,
    DateTime? CompletedDateTime);

public static class LessonCommentModelExtensions
{
    public static IEnumerable<LessonCommentModel> ConvertFromDtos(this IEnumerable<LessonCommentDto> lessonComments) =>
        lessonComments.Select(lc => new LessonCommentModel(
            lc.Content,
            lc.Completed,
            lc.StruckOut,
            lc.CompletedDateTime));
}
