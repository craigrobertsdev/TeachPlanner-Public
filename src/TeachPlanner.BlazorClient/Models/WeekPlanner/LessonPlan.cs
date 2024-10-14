using TeachPlanner.BlazorClient.Models.Resources;
using TeachPlanner.BlazorClient.Models.Subjects;
using TeachPlanner.Shared.Contracts.LessonPlans;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.BlazorClient.Models.WeekPlanner;

public class LessonPlan
{
    public LessonPlanId? LessonPlanId { get; set; }
    public Subject Subject { get; set; } = default!;
    public string PlanningNotes { get; set; } = string.Empty;
    public string PlanningNotesHtml { get; set; } = string.Empty;
    public List<LessonComment> Comments { get; set; } = [];
    public List<string> CurriculumCodes { get; set; } = [];
    public List<Resource> Resources { get; set; } = [];
    public int NumberOfPeriods { get; set; }
    public int StartPeriod { get; set; }
}

public static class LessonPlanExtensions
{
    public static LessonPlan ConvertFromDto(this LessonPlanDto lessonPlan)
    {
        return new LessonPlan
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
    }

    public static IEnumerable<LessonPlan> ConvertFromDtos(this IEnumerable<LessonPlanDto> periods)
    {
        return periods.Select(p => p.ConvertFromDto());
    }
}