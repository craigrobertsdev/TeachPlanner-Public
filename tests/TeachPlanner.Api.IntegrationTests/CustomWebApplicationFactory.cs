using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeachPlanner.Api.Database;
using TeachPlanner.Api.IntegrationTests.Helpers;

namespace TeachPlanner.Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var servicesToRemove = new []
            {
                typeof(DbContextOptions<ApplicationDbContext>),
            };
            
            foreach (var serviceType in servicesToRemove)
            {
                var service = services.SingleOrDefault(d => d.ServiceType == serviceType);
                services.Remove(service!);
            }

            var guid = Guid.NewGuid();
            var connectionString = $"server=localhost;User=test;password=testing123!;database=testdb-{guid}";
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            SeedData.PopulateTestData(context);
        });
    }

    protected override void Dispose(bool disposing)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureDeleted();
    }
}