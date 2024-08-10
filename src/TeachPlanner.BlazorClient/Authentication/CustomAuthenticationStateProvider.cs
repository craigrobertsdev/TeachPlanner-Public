using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace TeachPlanner.BlazorClient.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private static ClaimsPrincipal _anonymous => new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("JWT_KEY");
        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(_anonymous);
        }

        var claims = JwtHelpers.ParseClaimsFromJwt(token);
        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "JwtAuth")));
    }

    public async Task NotifyAuthStateAsync()
    {
        var authState = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public void NotifyAuthState()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
