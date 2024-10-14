using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.Users;
using TeachPlanner.Api.Services.Authentication;
using TeachPlanner.Shared.Contracts.Authentication;

namespace TeachPlanner.Api.IntegrationTests.Features.Authentication;

public class AuthenticationTests : IClassFixture<MySqlFixture>
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _dbContext;
    private readonly CustomWebApplicationFactory _factory;
    private readonly MySqlFixture _fixture;
    private static readonly ApplicationUser _applicationUser =
        new ApplicationUser { UserName = "test@mail.com", Email = "test@mail.com" };

    public AuthenticationTests(MySqlFixture fixture)
    {
        _client = fixture.CreateClient();
        _dbContext = fixture.CreateDbContext();
        _factory = fixture.Factory;
        _fixture = fixture;
    }

    [Fact]
    public async Task Register_WithValidCredentials_ShouldSucceed()
    {
        var authRequest = new RegisterModel()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@mail.com",
            Password = "Testing123!",
            ConfirmedPassword = "Testing123!"
        };

        var response = await _client.PostAsJsonAsync("api/authentication/register", authRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("testing")]
    [InlineData("Testing123")]
    [InlineData("Testing123456")]
    [InlineData("testing123!")]
    public async Task Register_WithInvalidPassword_ShouldFail(string password)
    {
        var registerRequest = new RegisterModel()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "invalidPassword@mail.com",
            Password = password,
            ConfirmedPassword = password
        };

        var response = await _client.PostAsJsonAsync("api/authentication/register", registerRequest);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Register_DuplicateUser_ShouldFail()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Register_DifferentPasswords_ShouldFail()
    {
        throw new NotImplementedException();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldSucceed()
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await userManager.CreateAsync(_applicationUser, "Testing123!");
        var user = await _dbContext.Users.Where(u => u.Email == _applicationUser.Email).FirstAsync();
        var teacher = Domain.Teachers.Teacher.Create(user.Id, "First", "Last");
        _dbContext.Teachers.Add(teacher);
        await _dbContext.SaveChangesAsync();

        var login = new LoginModel { Email = _applicationUser.Email!, Password = "Testing123!" };
        var response = await _client.PostAsJsonAsync("api/authentication/login", login);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [MemberData(nameof(InvalidLoginData))]
    public async Task Login_WithInvalidCredentials_ShouldFail(string email, string password)
    {
        var login = new LoginModel { Email = email, Password = password };

        var response = await _client.PostAsJsonAsync("api/authentication/login", login);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_WithValidToken_ShouldSucceed()
    {
        var response = await _client.GetAsync("api/authentication/me");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetMe_WithExpiredToken_ShouldFail()
    {
        var (user, teacher) = await CreateUser("expiredTokenValidRefresh");
        var expiredToken = GenerateExpiredToken(user, teacher);

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/authentication/me");
        request.Headers.Add("Authorization", $"Bearer {expiredToken}");
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    public static IEnumerable<object[]> InvalidLoginData()
    {
        yield return [_applicationUser.Email!, "ThisIsTheWrongPassword123!"];
        yield return ["nonexistentEmail@test.com", "ThisIsTheWrongPassword123!"];
    }

    private string GenerateExpiredToken(ApplicationUser user, Teacher teacher)
    {
        var scope = _factory.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        typeof(JwtSettings).GetProperty(nameof(JwtSettings.ExpiryMinutes))!.SetValue(jwtSettings, -1);
        var tokenGenerator = new JwtTokenGenerator(jwtSettings);
        var token = tokenGenerator.GenerateToken(teacher, user.Email!).Token;

        return token;

    }

    private async Task<(ApplicationUser user, Teacher teacher)> CreateUser(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var newUser = new ApplicationUser { UserName = email, Email = email };
        await userManager.CreateAsync(newUser, "Testing123!");
        var user = await _dbContext.Users.Where(u => u.Email == newUser.Email).FirstAsync();
        var teacher = Teacher.Create(user.Id, "First", "Last");
        _dbContext.Teachers.Add(teacher);
        await _dbContext.SaveChangesAsync();
        return (user, teacher);
    }
}