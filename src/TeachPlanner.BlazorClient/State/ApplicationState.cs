using Microsoft.JSInterop;
using TeachPlanner.BlazorClient.Authentication;
using TeachPlanner.Shared.Contracts.Teachers;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.BlazorClient.State;

public class ApplicationState
{
    private readonly IJSRuntime _jsRuntime;

    public ApplicationState(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
        Teacher = new TeacherStateContainer();
    }

    public TeacherStateContainer Teacher { get; }

    public void SetLastSelectedYear(int year)
    {
        Teacher.LastSelectedYear = year;
        NotifyStateChanged();
    }

    public void SetLastSelectedWeekStart(DateOnly date)
    {
        Teacher.LastSelectedWeekStart = date;
        NotifyStateChanged();
    }

    public async Task LoadState()
    {
        var token = await _jsRuntime.InvokeAsync<string?>("getTokenFromLocalStorage");
        if (token is null)
        {
            return;
        }

        var claims = JwtHelpers.ParseClaimsFromJwt(token).ToList();
        var teacherIdClaim = claims.FirstOrDefault(claim => claim.Type == "id");
        if (teacherIdClaim is not null)
        {
            Teacher.Id = new TeacherId(Guid.Parse(teacherIdClaim.Value));
        }

        var firstName = claims.FirstOrDefault(claim => claim.Type == "given_name");
        if (firstName is not null)
        {
            Teacher.FirstName = firstName.Value;
        }

        var accountSetupStatus = await _jsRuntime.InvokeAsync<bool>("getAccountSetupStatus");
        if (accountSetupStatus)
        {
            Teacher.AccountSetupComplete = accountSetupStatus;
        }

        var settings = await _jsRuntime.InvokeAsync<SettingsResponse>("getAccountSettings");
        Teacher.LastSelectedYear = settings.LastSelectedYear == 0 ? DateTime.Now.Year : settings.LastSelectedYear;

        var date = settings.LastSelectedWeekStart == default
            ? new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            : settings.LastSelectedWeekStart;
        
        if (date.DayOfWeek != DayOfWeek.Monday)
        {
            date = date.AddDays(-(int)date.DayOfWeek + 1);
        }

        Teacher.LastSelectedWeekStart = date;
    }

    public event Action? OnChange;

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}