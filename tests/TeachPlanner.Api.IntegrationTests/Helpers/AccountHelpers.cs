using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeachPlanner.Api.Domain.Teachers;
using TeachPlanner.Api.Domain.Users;

namespace TeachPlanner.Api.IntegrationTests.Helpers;

internal static class AccountHelpers
{
    internal static async Task<ApplicationUser> CreateUser(MySqlFixture fixture)
    {
        using var scope = fixture.Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var email = "TestUser@test.com";
        var result = await userManager.CreateAsync(new ApplicationUser()
        {
            Email = email,
            UserName = email,
        });

        await using var context = fixture.CreateDbContext();
        var user = await context.Users.FirstAsync(u => u.Email == email);
        return user;
    }

    internal static async Task<Teacher> CreateTeacher(MySqlFixture fixture, ApplicationUser user)
    {
        var teacher = Teacher.Create(user.Id, "Fred", "Smith");
        await using var context = fixture.CreateDbContext();
        context.Teachers.Add(teacher);
        await context.SaveChangesAsync();

        return teacher;

    }

}