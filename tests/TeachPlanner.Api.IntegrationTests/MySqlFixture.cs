using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.Domain.Users;
using TeachPlanner.Api.IntegrationTests.Helpers;
using TeachPlanner.Api.Services.Authentication;

namespace TeachPlanner.Api.IntegrationTests;

public class MySqlFixture : IDisposable
{
    public readonly CustomWebApplicationFactory Factory;
    private readonly string _token;
    public readonly ApplicationUser ApplicationUser;


    public MySqlFixture()
    {
        ApplicationUser = new ApplicationUser { UserName = "test@mail.com", Email = "test@mail.com" };
        Factory = new CustomWebApplicationFactory();
        using var scope = Factory.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        var tokenGenerator = new JwtTokenGenerator(jwtSettings);
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var teacher = dbContext.Teachers.First();
        _token = tokenGenerator.GenerateToken(teacher, ApplicationUser.Email).Token;
    }

    public HttpClient CreateClient()
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

        return client;
    }

    public ApplicationDbContext CreateDbContext()
    {
        var scopeFactory = Factory.Services.GetRequiredService<IServiceScopeFactory>();
        var scope = scopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
    
    public void ResetTestData(ApplicationDbContext context)
    {
        SeedData.PopulateTestData(context);
    }

    public void Dispose()
    {
        Factory.Dispose();
    }
}