using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using TeachPlanner.BlazorClient.Authentication;
using TeachPlanner.BlazorClient.State;
using TeachPlanner.Shared.Contracts.Authentication;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.BlazorClient.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IHttpClientFactory _factory;
    private readonly ILocalStorageService _localStorage;
    private readonly ApplicationState _applicationState;
    private readonly NavigationManager _navigationManager;

    private const string JWT_KEY = nameof(JWT_KEY);
    private const string REFRESH_KEY = nameof(REFRESH_KEY);

    private string? _jwtCache;

    public event Action<string?>? LoginChange;

    public AuthenticationService(IHttpClientFactory factory, ILocalStorageService localStorage, ApplicationState applicationState, NavigationManager navigationManager)
    {
        _factory = factory;
        _localStorage = localStorage;
        _applicationState = applicationState;
        _navigationManager = navigationManager;
    }

    public async ValueTask<string> GetJwt()
    {
        if (string.IsNullOrEmpty(_jwtCache))
            _jwtCache = await _localStorage.GetItemAsync<string>(JWT_KEY);

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

    private static string GetUsername(string token)
    {
        var jwt = new JwtSecurityToken(token);

        return jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;
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
            throw new UnauthorizedAccessException("Invalid email or password.");

        var content = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

        if (content == null)
            throw new InvalidDataException();

        await _localStorage.SetItemAsync(JWT_KEY, content.Token);
        await _localStorage.SetItemAsync(REFRESH_KEY, content.RefreshToken);
        await _localStorage.SetItemAsync("AccountSetupComplete", content.AccountSetupStatus);

        SetAppState(content);

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

    private void SetAppState(AuthenticationResponse content)
    {
        var claims = JwtHelpers.ParseClaimsFromJwt(content.Token);
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
}
