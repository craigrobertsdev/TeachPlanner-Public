using TeachPlanner.Shared.Domain.Common.Interfaces;
using TeachPlanner.Shared.Domain.Common.Primatives;
using TeachPlanner.Shared.Domain.Teachers;
using TeachPlanner.Shared.Domain.YearDataRecords;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.Shared.Domain.LessonPlans;

public sealed class LessonPlan : Entity<LessonPlanId>, IAggregateRoot
{
    private readonly List<LessonComment> _comments = [];
    private readonly List<Guid> _contentDescriptionIds = [];
    private readonly List<Resource> _resources = [];
    public YearDataId YearDataId { get; private set; }
    public SubjectId SubjectId { get; private set; }
    public string PlanningNotes { get; private set; }
    public string PlanningNotesHtml { get; private set; }
    public DateOnly LessonDate { get; private set; }
    public int NumberOfPeriods { get; private set; }
    public int StartPeriod { get; private set; }
    public DateTime CreatedDateTime { get; private set; }
    public DateTime UpdatedDateTime { get; private set; }
    public IReadOnlyList<Resource> Resources => _resources.AsReadOnly();
    public IReadOnlyList<LessonComment> Comments => _comments.AsReadOnly();
    public IReadOnlyList<Guid> ContentDescriptionIds => _contentDescriptionIds.AsReadOnly();


    public void AddLessonComment(LessonComment comment)
    {
        if (!_comments.Contains(comment))
        {
            _comments.Add(comment);
            UpdatedDateTime = DateTime.Now;
        }
    }

    public void AddResource(Resource resource)
    {
        if (!_resources.Contains(resource))
        {
            _resources.Add(resource);
            UpdatedDateTime = DateTime.Now;
        }
    }

    public void SetNumberOfPeriods(int newNumberOfPeriods)
    {
        if (newNumberOfPeriods != NumberOfPeriods)
        {
            NumberOfPeriods = newNumberOfPeriods;
            UpdatedDateTime = DateTime.Now;
        }
    }

    public void SetPlanningNotes(string newPlanningNotes, string newPlanningNotesHtml) => (PlanningNotes, PlanningNotesHtml) = (newPlanningNotes, newPlanningNotesHtml);

    public void AddCurriculumCodes(IEnumerable<Guid> contentDescriptionIds)
    {
        _contentDescriptionIds.Clear();
        _contentDescriptionIds.AddRange(contentDescriptionIds);
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

    public void ClearResources() => _resources.Clear();

    public IEnumerable<Resource> MatchResources(IEnumerable<Resource> resources) =>
        resources.Where(_resources.Contains);

    public static LessonPlan Create(
        YearDataId yearDataId,
        SubjectId subjectId,
        List<Guid> contentDescriptionIds,
        string planningNotes,
        string planningNotesHtml,
        int numberOfPeriods,
        int startPeriod,
        DateOnly lessonDate,
        List<Resource>? resources)
    {
        if (startPeriod < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(startPeriod), "Start period must be greater than 0");
        }

        if (numberOfPeriods < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfPeriods), "Number of periods must be greater than 0");
        }
        return new LessonPlan(
            new LessonPlanId(Guid.NewGuid()),
            yearDataId,
            subjectId,
            contentDescriptionIds,
            planningNotes,
            planningNotesHtml,
            numberOfPeriods,
            startPeriod,
            lessonDate,
            DateTime.UtcNow,
            DateTime.UtcNow,
            resources);
    }
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
        NumberOfPeriods = numberOfPeriods;
        StartPeriod = startPeriod;
        LessonDate = lessonDate;
        CreatedDateTime = createdDateTime;
        UpdatedDateTime = updatedDateTime;
        _resources = resources ?? [];
    }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private LessonPlan()
    {
    }
}