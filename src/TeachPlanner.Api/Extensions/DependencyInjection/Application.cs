using System.Reflection;
using FluentValidation;
using TeachPlanner.Shared.Domain.Teachers;

namespace TeachPlanner.Api.Extensions.DependencyInjection;

public static class Application
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddPipeline();
        services.AddConverters();

        return services;
    }

    public static IServiceCollection AddPipeline(this IServiceCollection services)
    {

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.RegisterServicesFromAssemblies(typeof(Teacher).Assembly);
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IServiceCollection AddConverters(this IServiceCollection services)
    {
        services.AddDateOnlyTimeOnlyStringConverters();

        return services;
    }
}