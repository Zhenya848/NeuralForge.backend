using Microsoft.Extensions.DependencyInjection;
using NeuralForge.Application.Abstractions;

namespace NeuralForge.Application;

public static class Inject
{
    public static IServiceCollection AddFromApplication(this IServiceCollection services)
    {
        var assembly = typeof(Inject).Assembly;
        
        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableToAny(
                typeof(ICommandHandler<,>),
                typeof(IQueryHandler<,>)))
            .AsSelfWithInterfaces()
            .WithLifetime(ServiceLifetime.Scoped));

        return services;
    }
}