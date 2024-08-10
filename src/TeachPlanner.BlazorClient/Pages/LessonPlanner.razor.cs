using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MudBlazor;
using System.Net.Http.Json;
using TeachPlanner.BlazorClient.Components.LessonPlannerPage;
using TeachPlanner.BlazorClient.Components.Shared;
using TeachPlanner.BlazorClient.Models.Resources;
using TeachPlanner.BlazorClient.Models.Subjects;
using TeachPlanner.BlazorClient.Models.WeekPlanner;
using TeachPlanner.Shared.Common.Extensions;
using TeachPlanner.Shared.Contracts.Curriculum;
using TeachPlanner.Shared.Contracts.LessonPlans;
using TeachPlanner.Shared.Contracts.LessonPlans.CreateLessonPlan;
using TeachPlanner.Shared.Domain.Common.Enums;
using TeachPlanner.Shared.Domain.Curriculum;

namespace TeachPlanner.BlazorClient.Pages;

public partial class LessonPlanner : IDisposable
{
    [SupplyParameterFromQuery(Name = "date")] public DateOnly Date { get; set; }
    [SupplyParameterFromQuery(Name = "periodNumber")] public int PeriodNumber { get; set; }
    [SupplyParameterFromQuery(Name = "isNewLesson")] public bool IsNewLesson { get; set; }

    private HttpClient _client = null!;
    private string? _error = null;
    private bool _changesSaved = false;

    // These are used to track changes to the lesson plan.
    private string _originalSubjectName = string.Empty;
    private List<string> _originalContentDescriptions = [];
    private List<ResourceModel> _originalResources = [];
    public string PlanningNotes = string.Empty;
    public string PlanningNotesHtml = string.Empty;
    public LessonPlanModel LessonPlan { get; set; } = default!;
    private int NumberOfPeriods { get; set; } = 1;
    private SubjectData CurrentSubject { get; set; } = default!;
    private List<SubjectModel> CurriculumSubjects { get; set; } = [];
    private readonly List<SubjectData> _cache = [];
    private List<ContentDescriptionEntry> _selectedContentDescriptionListEntries = [];
    private List<YearLevelValue> YearLevelsTaught { get; set; } = [];
    private IDisposable? _registration;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _registration = NavManager.RegisterLocationChangingHandler(HandleNavigateAway);
            await SetContainerHeights();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        _client = Http.CreateClient("ServerApi");
        try
        {
            if (IsNewLesson)
            {
                var response = await _client.GetFromJsonAsync<LessonPlanResponse>(
                    $"api/{AppState.Teacher.Id.Value}/lesson-plans?lessonDate={Date.Year}/{Date.Month}/{Date.Day}&period={PeriodNumber}&isNewLesson={IsNewLesson}")
                    ?? throw new Exception("Failed to retrieve data from the server");

                LessonPlan = new();
                CurriculumSubjects = response.Curriculum.ConvertFromDtos();
                CurriculumSubjects.Sort((s1, s2) => s1.Name.CompareTo(s2.Name));
                CurrentSubject = new SubjectData(CurriculumSubjects[0].SubjectId, CurriculumSubjects[0].Name);
                _originalSubjectName = CurrentSubject.Name;
            }
            else
            {
                var response = await _client.GetFromJsonAsync<LessonPlanResponse>(
                    $"api/{AppState.Teacher.Id.Value}/lesson-plans?lessonDate={Date}&period={PeriodNumber}&isNewLesson={IsNewLesson}")
                    ?? throw new Exception("Failed to retrieve data from the server");

                LessonPlan = response.LessonPlan!.ConvertFromDto();
                CurrentSubject = InitialiseSubjectData(response.LessonPlan!.Subject);
                CurriculumSubjects = [LessonPlan.Subject];
                NumberOfPeriods = LessonPlan.NumberOfPeriods;
                PeriodNumber = LessonPlan.StartPeriod;
                PlanningNotesHtml = LessonPlan.PlanningNotesHtml;
                _originalSubjectName = CurrentSubject.Name;
                _originalContentDescriptions = CurrentSubject.SelectedContentDescriptions.Values.SelectMany(cd => cd.SelectMany(cd => cd.CurriculumCodes)).ToList();
                _originalResources = [.. LessonPlan.Resources];
                _selectedContentDescriptionListEntries = GenerateContentDescriptionListEntries(CurrentSubject.SelectedContentDescriptions);
            }

            YearLevelsTaught = await _client.GetFromJsonAsync<List<YearLevelValue>>(
                $"api/{AppState.Teacher.Id.Value}/year-levels-taught/?calendarYear={Date.Year}")
                ?? throw new Exception("Failed to retrieve data from the server");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private async Task SetContainerHeights()
    {
        await JSRuntime.InvokeVoidAsync("setContainerHeights");
    }

    private SubjectData InitialiseSubjectData(CurriculumSubjectDto subject) {
        // ContentDescriptions and Resources will be fetched if either dialog is opened, so they can be empty lists here.
        return new SubjectData(
            new SubjectId(subject.Id),
            subject.Name,
            [],
            LessonPlan.Subject.YearLevels.GetContentDescriptions(),
            [],
            LessonPlan.Resources);
    }

    private void HandleSubjectChange(Microsoft.AspNetCore.Components.ChangeEventArgs args)
    {
        var subject = (string)args.Value!;
        if (CurrentSubject!.Name == subject)
        {
            return;
        }

        _cache.Add(CurrentSubject);
        CurrentSubject = _cache.FirstOrDefault(cd => cd.Name == subject) ?? new(CurriculumSubjects.First(s => s.Name == subject).SubjectId, subject, [], [], [], []);
        _selectedContentDescriptionListEntries = GenerateContentDescriptionListEntries(CurrentSubject.SelectedContentDescriptions);
    }

    private async Task HandleSave()
    {
        _error = null;

        if (await OverlapExists())
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, NoHeader = true };
            var parameters = new DialogParameters<ConfirmDialog> {
                { x => x.Title, "A lesson overlap exists" },
                { x => x.Message, "The number of periods for this lesson creates an overlap with the next lesson." }
            };
            var dialog = await DialogService.ShowAsync<ConfirmDialog>(
                "A lesson overlap exists",
                parameters,
                options);
            var result = await dialog.Result;

            if (result.Canceled)
            {
                return;
            }
        }

        var lessonPlanRequest = new CreateLessonPlanRequest(
            CurrentSubject.Id.Value,
            CurrentSubject.SelectedContentDescriptions.Values.SelectMany(cd => cd.Select(cd => cd.Id)).ToList(),
            PlanningNotes,
            PlanningNotesHtml,
            CurrentSubject.SelectedResources.Select(r => r.Id.Value).ToList(),
            Date,
            NumberOfPeriods,
            PeriodNumber);

        var response = await _client.PostAsJsonAsync($"api/{AppState.Teacher.Id.Value}/lesson-plans", lessonPlanRequest);
        if (response.IsSuccessStatusCode)
        {
            _changesSaved = true;
            NavManager.NavigateTo("/week-planner");
        }
        else
        {
            _error = response.ReasonPhrase;
        }
    }

    private async Task<bool> PromptOnUnsavedChanges()
    {
        if (UnsavedChanges())
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true, NoHeader = true };
            var parameters = new DialogParameters<ConfirmDialog> {
                { x => x.Title, "You have unsaved changes" },
                { x => x.Message, "Are you sure you want to leave this page?" }
            };
            var result = await DialogService.Show<ConfirmDialog>("Unsaved changes", parameters, options).Result;

            if (result.Canceled)
            {
                return false;
            }
        }

        return true;
    }

    private bool UnsavedChanges()
    {
        return PlanningNotesHtml != LessonPlan.PlanningNotesHtml
            || _originalSubjectName != CurrentSubject.Name
            || !_originalContentDescriptions.ContainsAll(CurrentSubject.SelectedContentDescriptions.Values.SelectMany(cd => cd.SelectMany(cd => cd.CurriculumCodes)))
            || !_originalResources.ContainsAll(CurrentSubject.SelectedResources);

    }

    private async Task OpenResourcesDialog()
    {
        var currentResourceSelection = CurrentSubject.SelectedResources.ToList();
        var parameters = new DialogParameters<ResourcesDialog> {
            { x => x.Resources, CurrentSubject.Resources },
            { x => x.SelectedResources, CurrentSubject.SelectedResources },
            { x => x.SubjectId, CurrentSubject.Id }
        };
        var options = new DialogOptions { CloseButton = false, CloseOnEscapeKey = false };
        var result = await DialogService.Show<ResourcesDialog>("Add Resources", parameters, options).Result;
        var data = ((bool, List<ResourceModel>, List<ResourceModel>))result.Data;
        if (data.Item1 == false)
        {
            // if dialog is cancelled, we only want to cache the result of getting the resources from the server
            CurrentSubject.Resources = data.Item2;
            CurrentSubject.SelectedResources = currentResourceSelection;
            return;
        }

        (CurrentSubject.Resources, CurrentSubject.SelectedResources) = (data.Item2, data.Item3);
    }

    private async Task OpenContentDescriptionsDialog()
    {
        var key = CurrentSubject.SelectedContentDescriptions.Keys.FirstOrDefault();
        var value = CurrentSubject.SelectedContentDescriptions.Values.FirstOrDefault();
        // In testing sometimes modifying selecting content descriptions would change the underlying list, sometimes not.
        // This is to ensure we have a copy of the original list
        var currentContentDescriptionSelection = CurrentSubject.SelectedContentDescriptions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());

        var parameters = new DialogParameters<ContentDescriptionsDialog> {
            { x => x.YearLevels, YearLevelsTaught },
            { x => x.SubjectId, CurriculumSubjects.First(s => s.Name == CurrentSubject.Name)!.SubjectId },
            { x => x.ContentDescriptions, CurrentSubject.ContentDescriptions },
            { x => x.SelectedContentDescriptions, CurrentSubject.SelectedContentDescriptions }
        };
        var options = new DialogOptions { CloseButton = false, CloseOnEscapeKey = false };
        var result = await DialogService.Show<ContentDescriptionsDialog>
            ("Add Content Descriptions", parameters, options).Result;
        var data = ((bool, Dictionary<YearLevelValue, List<ContentDescriptionModel>>, Dictionary<YearLevelValue, List<ContentDescriptionModel>>))result.Data;

        if (data.Item1 == false)
        {
            // if dialog is cancelled, we only want to cache the result of getting the content descriptions from the server
            CurrentSubject.ContentDescriptions = data.Item2;
            CurrentSubject.SelectedContentDescriptions = currentContentDescriptionSelection;
            return;
        }

        (CurrentSubject.ContentDescriptions, CurrentSubject.SelectedContentDescriptions) = (data.Item2, data.Item3);
        _selectedContentDescriptionListEntries = GenerateContentDescriptionListEntries(CurrentSubject.SelectedContentDescriptions);
    }

    private List<ContentDescriptionEntry> GenerateContentDescriptionListEntries(Dictionary<YearLevelValue, List<ContentDescriptionModel>> selectedContentDescriptions)
    {
        var keys = selectedContentDescriptions.Keys.ToList();
        var values = selectedContentDescriptions.Values.FirstOrDefault();
        return selectedContentDescriptions
            .SelectMany(kvp => kvp.Value
                .Select(cd => new ContentDescriptionEntry(
                    cd.Id,
                    cd.CurriculumCodes,
                    cd.Text,
                    kvp.Key.ToDisplayString(),
                    () => RemoveContentDescription(kvp.Key, cd.Id))))
            .ToList();
    }

    private void RemoveContentDescription(YearLevelValue key, Guid contentDescriptionId)
    {
        CurrentSubject.SelectedContentDescriptions[key] = CurrentSubject.SelectedContentDescriptions[key]
            .Where(cd => cd.Id != contentDescriptionId).ToList();
        _selectedContentDescriptionListEntries = _selectedContentDescriptionListEntries.Where(cd => cd.Id != contentDescriptionId).ToList();
    }

    private async Task<bool> OverlapExists()
    {
        var lessonPlanIdQuery = LessonPlan.LessonPlanId is not null ? $"lessonPlanId={LessonPlan.LessonPlanId.Value}&" : "";
        return await _client.GetFromJsonAsync<bool>(
            $"api/{AppState.Teacher.Id.Value}/lesson-plans/check-overlap?{lessonPlanIdQuery}lessonDate={Date}&lessonNumber={PeriodNumber}&numberOfPeriods={NumberOfPeriods}");
    }

    private async Task HandleLeavePage()
    {
        await JSRuntime.InvokeVoidAsync("history.back");
    }

    private async ValueTask HandleNavigateAway(LocationChangingContext context)
    {
        if (!_changesSaved)
        {
            var proceed = await PromptOnUnsavedChanges();
            if (!proceed)
            {
                context.PreventNavigation();
            }
        }
    }
    
    public void Dispose()
    {
        _registration?.Dispose();
    }

    record ContentDescriptionEntry(Guid Id, List<string> CurriculumCodes, string ContentDescription, string YearLevel, Action OnClick);

    class SubjectData
    {
        public SubjectId Id { get; set; }
        public string Name { get; set; }
        public Dictionary<YearLevelValue, List<ContentDescriptionModel>> ContentDescriptions { get; set; }
        public Dictionary<YearLevelValue, List<ContentDescriptionModel>> SelectedContentDescriptions { get; set; }
        public List<ResourceModel> Resources { get; set; }
        public List<ResourceModel> SelectedResources { get; set; }

        public SubjectData(
            SubjectId id,
            string name,
            Dictionary<YearLevelValue, List<ContentDescriptionModel>> contentDescriptions,
            Dictionary<YearLevelValue, List<ContentDescriptionModel>> selectedContentDescriptions,
            List<ResourceModel> resources,
            List<ResourceModel> selectedResources)
        {
            Id = id;
            Name = name;
            ContentDescriptions = contentDescriptions;
            SelectedContentDescriptions = selectedContentDescriptions;
            Resources = resources;
            SelectedResources = selectedResources;
        }

        public SubjectData(SubjectId id, string name)
        {
            Id = id;
            Name = name;
            ContentDescriptions = [];
            SelectedContentDescriptions = [];
            Resources = [];
            SelectedResources = [];
        }
    }
}
