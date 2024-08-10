using TeachPlanner.BlazorClient.Models.Resources;
using TeachPlanner.BlazorClient.Models.Subjects;
using TeachPlanner.Shared.Contracts.LessonPlans;
using TeachPlanner.Shared.Domain.LessonPlans;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public class LessonPlanModel
{
    public LessonPlanId? LessonPlanId { get; set; } = default!;
    public SubjectModel Subject { get; set; } = default!;
    public string PlanningNotes { get; set; } = string.Empty;
    public string PlanningNotesHtml { get; set; } = string.Empty;
    public List<LessonCommentModel> Comments { get; set; } = [];
    public List<string> CurriculumCodes { get; set; } = [];
    public List<ResourceModel> Resources { get; set; } = [];
    public int NumberOfPeriods { get; set; }
    public int StartPeriod { get; set; }
}

public static class LessonPlanModelExtensions
{
    public static LessonPlanModel ConvertFromDto(this LessonPlanDto lessonPlan) =>
        new()
        {
            LessonPlanId = new LessonPlanId(lessonPlan.LessonPlanId),
            Subject = lessonPlan.Subject.ConvertFromDto(),
            PlanningNotes = lessonPlan.PlanningNotes,
            PlanningNotesHtml = lessonPlan.PlanningNotesHtml,
            NumberOfPeriods = lessonPlan.NumberOfPeriods,
            StartPeriod = lessonPlan.StartPeriod,
            Comments = lessonPlan.Comments?.ConvertFromDtos().ToList() ?? [],
            Resources = lessonPlan.Resources?.ConvertFromDtos().ToList() ?? []
        };

    public static IEnumerable<LessonPlanModel> ConvertFromDtos(this IEnumerable<LessonPlanDto> periods) =>
        periods.Select(p => p.ConvertFromDto());
}
