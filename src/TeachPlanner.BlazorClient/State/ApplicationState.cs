using Microsoft.JSInterop;
using TeachPlanner.BlazorClient.Authentication;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.BlazorClient.State;

public class ApplicationState
{
    private readonly IJSRuntime _jsRuntime;

    public ApplicationState(IJSRuntime jSRuntime)
    {
        _jsRuntime = jSRuntime;
        Teacher = new();
    }

    public TeacherStateContainer Teacher { get; }

    public void SetTeacherId(TeacherId teacherId)
    {
        Teacher.Id = teacherId;
        NotifyStateChanged();
    }

    public void SetTeacherFirstName(string firstName)
    {
        Teacher.FirstName = firstName;
        NotifyStateChanged();
    }

    public async Task LoadState()
    {
        var token = await _jsRuntime.InvokeAsync<string>("getTokenFromLocalStorage");
        if (token is null)
        {
            return;
        }

        var claims = JwtHelpers.ParseClaimsFromJwt(token);
        var teacherId = claims.FirstOrDefault(claim => claim.Type == "id");
        if (teacherId is not null)
        {
            Teacher.Id = new TeacherId(Guid.Parse(teacherId.Value));
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
    }

    public event Action? OnChange;
    private void NotifyStateChanged() => OnChange?.Invoke();
}
