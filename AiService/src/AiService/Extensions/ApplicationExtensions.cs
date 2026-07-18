using System.Reflection;
using AiService.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AiService.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddEndpoints(assembly);
        services.AddHandlers(assembly);

        return services;
    }

    private static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly)
    {
        var endpoints = assembly.DefinedTypes
            .Where(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                t.IsAssignableTo(typeof(IEndpoint)));

        foreach (var endpoint in endpoints)
        {
            services.AddTransient(typeof(IEndpoint), endpoint);
        }

        return services;
    }

    private static IServiceCollection AddHandlers(
        this IServiceCollection services,
        Assembly assembly)
    {
        var handlers = assembly.DefinedTypes
            .Where(t =>
                !t.IsAbstract &&
                !t.IsInterface &&
                t.IsAssignableTo(typeof(ICommandHandler)));

        foreach (var handler in handlers)
        {
            services.AddScoped(handler.AsType());
        }

        return services;
    }

    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? group = null)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = group is null ? app : group;

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}