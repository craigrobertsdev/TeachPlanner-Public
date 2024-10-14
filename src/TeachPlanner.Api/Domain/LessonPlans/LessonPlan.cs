using TeachPlanner.Api.Domain.Common.Interfaces;
using TeachPlanner.Api.Domain.Common.Primatives;
using TeachPlanner.Api.Domain.Curriculum;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Contracts.LessonPlans;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.Api.Domain.LessonPlans;

public sealed class LessonPlan : Entity<LessonPlanId>, IAggregateRoot
{
    private readonly List<LessonComment> _comments = [];
    private readonly List<Guid> _contentDescriptionIds = [];
    private readonly List<Resource> _resources = [];

    private LessonPlan(
        LessonPlanId id,
        YearDataId yearDataId,
        SubjectId subjectId,
        List<Guid> contentDescriptionIds,
        string planningNotes,
        string planningNotesHtml,
        int numberOfPeriods,
        int startPeriod,
        DateOnly lessonDate,
        DateTime createdDateTime,
        DateTime updatedDateTime,
        List<Resource>? resources) : base(id)
    {
        YearDataId = yearDataId;
        SubjectId = subjectId;
        _contentDescriptionIds = contentDescriptionIds;
        PlanningNotes = planningNotes;
        PlanningNotesHtml = planningNotesHtml;
        NumberOfLessons = numberOfPeriods;
        StartPeriod = startPeriod;
        LessonDate = lessonDate;
        CreatedDateTime = createdDateTime;
        UpdatedDateTime = updatedDateTime;
        _resources = resources ?? [];
    }

    public YearDataId YearDataId { get; private set; }
    public SubjectId SubjectId { get; private set; }
    public string PlanningNotes { get; private set; }
    public string PlanningNotesHtml { get; private set; }
    public DateOnly LessonDate { get; private set; }
    public int NumberOfLessons { get; private set; }
    public int StartPeriod { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }
    public IReadOnlyList<Resource> Resources => _resources.AsReadOnly();
    public IReadOnlyList<LessonComment> Comments => _comments.AsReadOnly();
    public IReadOnlyList<Guid> ContentDescriptionIds => _contentDescriptionIds.AsReadOnly();

    public void AddResource(Resource resource)
    {
        if (!_resources.Contains(resource))
        {
            _resources.Add(resource);
            UpdatedDateTime = DateTime.Now;
        }
    }

    public void SetNumberOfLessons(int newNumberOfLessons)
    {
        if (newNumberOfLessons == NumberOfLessons)
        {
            return;
        }

        NumberOfLessons = newNumberOfLessons;
        UpdatedDateTime = DateTime.Now;
    }

    public void SetStartPeriod(int startPeriod)
    {
        if (startPeriod == StartPeriod)
        {
            return;
        }

        StartPeriod = startPeriod;
        UpdatedDateTime = DateTime.Now;
    }
    
    public void SetCurriculumCodes(List<Guid> contentDescriptionIds)
    {
        _contentDescriptionIds.Clear();
        _contentDescriptionIds.AddRange(contentDescriptionIds);
        UpdatedDateTime = DateTime.Now;
    }

    public void UpdateSubject(SubjectId subjectId)
    {
        if (SubjectId == subjectId)
        {
            return;
        }

        SubjectId = subjectId;
        UpdatedDateTime = DateTime.Now;
    }

    public void SetPlanningNotes(string newPlanningNotes, string newPlanningNotesHtml)
    {
        (PlanningNotes, PlanningNotesHtml) = (newPlanningNotes, newPlanningNotesHtml);
    }

    public void UpdateResources(IEnumerable<Resource> resources)
    {
        if (!resources.Any())
        {
            _resources.Clear();
            return;
        }

        var resourcesToRemove = _resources.Where(r => !resources.Contains(r)).ToList();
        var resourcesToAdd = resources.Where(r => !_resources.Contains(r)).ToList();
        _resources.RemoveAll(resourcesToRemove.Contains);
        _resources.AddRange(resourcesToAdd);
    }

    public void ClearResources()
    {
        _resources.Clear();
    }

    public IEnumerable<Resource> MatchResources(IEnumerable<Resource> resources)
    {
        return resources.Where(_resources.Contains);
    }

    public static LessonPlan Create(
        YearDataId yearDataId,
        SubjectId subjectId,
        List<Guid> contentDescriptionIds,
        string planningNotes,
        string planningNotesHtml,
        int numberOfLessons,
        int startPeriod,
        DateOnly lessonDate,
        List<Resource>? resources)
    {
        if (startPeriod < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startPeriod), "Start period must be greater than 0");
        }

        if (numberOfLessons < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfLessons), "Number of periods must be greater than 0");
        }

        return new LessonPlan(
            new LessonPlanId(Guid.NewGuid()),
            yearDataId,
            subjectId,
            contentDescriptionIds,
            planningNotes,
            planningNotesHtml,
            numberOfLessons,
            startPeriod,
            lessonDate,
            DateTime.UtcNow,
            DateTime.UtcNow,
            resources);
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private LessonPlan()
    {
    }
}

public static class LessonPlanDtoExtensions
{
    public static List<LessonCommentDto> ToDtos(this IEnumerable<LessonComment> comments)
    {
        return comments.Select(c => new LessonCommentDto(c.Content, c.Completed, c.StruckOut, c.CompletedDateTime))
            .ToList();
    }

    public static List<LessonPlanDto> ToDtos(this IEnumerable<LessonPlan> lessonPlans, IEnumerable<Resource> resources,
        IEnumerable<CurriculumSubject> subjects)
    {
        return lessonPlans.Select(lp => new LessonPlanDto(
                lp.Id.Value,
                lp.MatchSubject(subjects),
                lp.PlanningNotes,
                lp.PlanningNotesHtml,
                lp.MatchResources(resources).ConvertToDtos(),
                lp.Comments.ToDtos(),
                lp.StartPeriod,
                lp.NumberOfLessons))
            .ToList();
    }

    public static LessonPlanDto ToDto(this LessonPlan lessonPlan, IEnumerable<Resource> resources,
        CurriculumSubject subject)
    {
        var dto = new LessonPlanDto(
            lessonPlan.Id.Value,
            subject.ToDto(),
            lessonPlan.PlanningNotes,
            lessonPlan.PlanningNotesHtml,
            lessonPlan.MatchResources(resources).ConvertToDtos(),
            lessonPlan.Comments.ToDtos(),
            lessonPlan.StartPeriod,
            lessonPlan.NumberOfLessons);

        return dto;
    }

    public static CurriculumSubjectDto MatchSubject(this LessonPlan lessonPlan, IEnumerable<CurriculumSubject> subjects)
    {
        return subjects.First(s => s.Id == lessonPlan.SubjectId).ToDto();
    }
}