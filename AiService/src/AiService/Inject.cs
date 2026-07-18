using System.Security.Cryptography;
using AiService.Authorization;
using AiService.Db;
using AiService.Grpc.Interceptors;
using AiService.Models.AiMapping;
using AiService.Models.Options;
using AiService.Providers;
using AiService.Services;
using AiService.Services.Tokenizers;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AiService;

public static class Inject
{
    public static IServiceCollection AddFromAiService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();

        services.AddScoped<AppDbContext>();
        
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.Auth));
        services.AddOptions<AuthOptions>();
        
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

        services.Configure<AiSettings>(
            configuration.GetSection("AiSettings"));

        services.AddSingleton<AiRegistry>();
        services.AddHttpClient<AiProvider>();
        
        services.AddSingleton<ITokenizerService, TokenizerService>();
        
        services.AddSingleton<IsServiceInterceptor>();
        
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<IsServiceInterceptor>();
        });
        
        return services;
    }
}