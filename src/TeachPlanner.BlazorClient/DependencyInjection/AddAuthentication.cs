using Microsoft.AspNetCore.Components.Authorization;
using TeachPlanner.BlazorClient.Authentication;

namespace TeachPlanner.BlazorClient.DependencyInjection;

public static class AddAuthentication
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        services.AddAuthorizationCore();
        //services.AddApiAuthorization();

        return services;
    }
}