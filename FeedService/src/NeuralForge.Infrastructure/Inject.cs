using System.Security.Cryptography;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NeuralForge.Application.Abstractions;
using NeuralForge.Application.Repositories;
using NeuralForge.Infrastructure.Authorization;
using NeuralForge.Infrastructure.DbContexts;
using NeuralForge.Infrastructure.Repositories;
using NeuralForge.Presentation.Options;

namespace NeuralForge.Infrastructure;

public static class Inject
{
    public static IServiceCollection AddFromInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<AppDbContext>();
        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
        
        var infrastructureAssembly = typeof(Inject).Assembly;
        var applicationAssembly = typeof(IRepository).Assembly;
        
        services.Scan(scan => scan.FromAssemblies(infrastructureAssembly, applicationAssembly)
            .AddClasses(classes => classes.AssignableToAny(
                typeof(IRepository)))
            .AsSelfWithInterfaces()
            .WithLifetime(ServiceLifetime.Scoped));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        var authOptions = configuration.GetSection(AuthOptions.Auth).Get<AuthOptions>()
                          ?? throw new ApplicationException("Auth options not found");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var rsa = RSA.Create();

            byte[] publicKeyBytes = File.ReadAllBytes(authOptions.PublicKeyPath);
            rsa.ImportRSAPublicKey(publicKeyBytes, out _);

            var key = new RsaSecurityKey(rsa);

            options.TokenValidationParameters = TokenValidationParametersFactory
                .CreateWithLifeTime(key);
        });
        
        return services;
    }
}