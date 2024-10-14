using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using TeachPlanner.BlazorClient.Authentication;
using TeachPlanner.BlazorClient.State;
using TeachPlanner.Shared.Contracts.Authentication;
using TeachPlanner.Shared.Contracts.Teachers;
using TeachPlanner.Shared.StronglyTypedIds;

namespace TeachPlanner.BlazorClient.Services;

public class AuthenticationService : IAuthenticationService
{
    private const string JWT_KEY = nameof(JWT_KEY);
    private const string REFRESH_KEY = nameof(REFRESH_KEY);
    private readonly ApplicationState _applicationState;
    private readonly IHttpClientFactory _factory;
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigationManager;

    private string? _jwtCache;

    public AuthenticationService(IHttpClientFactory factory, ILocalStorageService localStorage,
        ApplicationState applicationState, NavigationManager navigationManager)
    {
        _factory = factory;
        _localStorage = localStorage;
        _applicationState = applicationState;
        _navigationManager = navigationManager;
    }

    public event Action<string?>? LoginChange;

    public async ValueTask<string> GetJwt()
    {
        if (string.IsNullOrEmpty(_jwtCache))
        {
            _jwtCache = await _localStorage.GetItemAsync<string>(JWT_KEY);
        }

        return _jwtCache;
    }

    public async Task Logout()
    {
        var response = await _factory.CreateClient("ServerApi").DeleteAsync("api/authentication/revoke");

        await _localStorage.RemoveItemAsync(JWT_KEY);
        await _localStorage.RemoveItemAsync(REFRESH_KEY);
        await _localStorage.RemoveItemAsync("AccountSetupComplete");

        _jwtCache = null;

        await Console.Out.WriteLineAsync($"Revoke gave response {response.StatusCode}");

        LoginChange?.Invoke(null);
    }

    public async Task Register(RegisterModel model)
    {
        var response = await _factory.CreateClient("ServerApi").PostAsync("api/authentication/register",
            JsonContent.Create(model));

        if (!response.IsSuccessStatusCode)
        {
            throw new UnauthorizedAccessException("Registration failed.");
        }

        var content = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();
        if (content is null)
        {
            throw new InvalidDataException();
        }

        await _localStorage.SetItemAsync(JWT_KEY, content.Token);
        await _localStorage.SetItemAsync(REFRESH_KEY, content.RefreshToken);

        SetAppState(content);
    }

    public async Task Login(LoginModel model)
    {
        var response = await _factory.CreateClient("ServerApi").PostAsync("api/authentication/login",
            JsonContent.Create(model));

        if (!response.IsSuccessStatusCode)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var content = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

        if (content == null)
        {
            throw new InvalidDataException();
        }

        await _localStorage.SetItemAsync(JWT_KEY, content.Token);
        await _localStorage.SetItemAsync(REFRESH_KEY, content.RefreshToken);
        await _localStorage.SetItemAsync("AccountSetupComplete", content.AccountSetupStatus);
        SetAppState(content);
        
        var settingsResponse = await _factory.CreateClient("ServerApi")
            .GetAsync($"api/{_applicationState.Teacher.Id.Value}/settings");
        
        var settings = await settingsResponse.Content.ReadFromJsonAsync<SettingsResponse>();
        if (settings is null)
        {
            throw new Exception("Failed to get data from the server. Please try again, or if the problem persists, contact support.");
        }

        await _localStorage.SetItemAsync("settings", settings);
        await SetTeacherSettings(settings);
        
        LoginChange?.Invoke(GetUsername(content.Token));
    }

    public async Task Refresh()
    {
        var accessToken = await _localStorage.GetItemAsync<string>(JWT_KEY);
        var refreshToken = await _localStorage.GetItemAsync<string>(REFRESH_KEY);
        var model = new RefreshModel(accessToken, refreshToken);

        var response = await _factory.CreateClient("ServerApi").PostAsync("api/authentication/refresh",
            JsonContent.Create(model));

        if (!response.IsSuccessStatusCode)
        {
            await Logout();
            _navigationManager.NavigateTo("/authentication/login", true);
        }

        var content = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

        if (content == null)
        {
            throw new InvalidDataException();
        }

        await _localStorage.SetItemAsync(JWT_KEY, content.Token);
        await _localStorage.SetItemAsync(REFRESH_KEY, content.RefreshToken);

        _jwtCache = content.Token;
    }

    public async Task DeleteAccount()
    {
        var teacherId = _applicationState.Teacher.Id;
        var response = await _factory.CreateClient("ServerApi").DeleteAsync($"api/{teacherId.Value}");
        if (response.IsSuccessStatusCode)
        {
            await _localStorage.ClearAsync();
        }
    }

    private static string GetUsername(string token)
    {
        var jwt = new JwtSecurityToken(token);

        return jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;
    }

    private void SetAppState(AuthenticationResponse content)
    {
        var claims = JwtHelpers.ParseClaimsFromJwt(content.Token).ToList();
        var teacherId = claims.FirstOrDefault(claim => claim.Type == "id");
        if (teacherId is not null)
        {
            _applicationState.Teacher.Id = new TeacherId(Guid.Parse(teacherId.Value));
        }

        var firstName = claims.FirstOrDefault(claim => claim.Type == "given_name");
        if (firstName is not null)
        {
            _applicationState.Teacher.FirstName = firstName.Value;
        }

        _applicationState.Teacher.AccountSetupComplete = content.AccountSetupStatus;
    }

    private async Task SetTeacherSettings(SettingsResponse settings)
    {
        await _localStorage.SetItemAsync("lastSelectedYear", settings.LastSelectedYear);
        await _localStorage.SetItemAsync("lastSelectedWeekStart", settings.LastSelectedWeekStart);
        
        _applicationState.SetLastSelectedYear(settings.LastSelectedYear);
        _applicationState.SetLastSelectedWeekStart(settings.LastSelectedWeekStart);
    }
}