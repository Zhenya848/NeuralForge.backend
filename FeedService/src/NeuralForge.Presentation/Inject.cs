using ActsValidator.Presentation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeuralForge.Application.Abstractions;
using NeuralForge.Presentation.Grpc.Interceptors;
using NeuralForge.Presentation.Grpc.Services;
using NeuralForge.Presentation.Options;

namespace NeuralForge.Presentation;

public static class Inject
{
    public static IServiceCollection AddFromPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.Auth));
        services.AddOptions<AuthOptions>();
        
        services.AddSingleton<ProvideSecretKeyInterceptor>();
        
        services.AddGrpcClient<Greeter.GreeterClient>(options =>
            {
                options.Address = new Uri("http://localhost:5245");
            })
            .AddInterceptor<ProvideSecretKeyInterceptor>();
        
        services.AddScoped<IGreeterService, GreeterService>();
        
        return services;
    }
}