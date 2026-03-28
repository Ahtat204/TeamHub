using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Helpers;
using TeamcollborationHub.server.Middlewares;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using TeamcollborationHub.server.Services.Caching;
using TeamcollborationHub.server.Services.Security;

namespace TeamcollborationHub.server.Services;

public static class Register
{
    public static IServiceCollection RegisterServices(this IServiceCollection services,IConfiguration configuration)
    {
        string sqlserver = LoadValues.LoadValue("sqlserverconnectionstring", configuration) ??
                           configuration.GetConnectionString("sqlserverconnectionstring") ??
                           throw new InvalidOperationException("SQL Server Connection string wasn't not found.");
        string redis = LoadValues.LoadValue("RedisConnectionString", configuration) ?? configuration.GetConnectionString("RedisConnectionString") ?? throw new InvalidOperationException("Redis Connection string  wasn't found .");
services.AddExceptionHandler<GlobalExceptionHandler>();
services.AddProblemDetails();
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
services.AddSwaggerGen();
services.AddScoped<ICachingService<Project, string>, RedisCachingService>();
services.AddScoped<ICachingService<RefreshToken, string>, RefreshTokenCachingService>();
services.AddSingleton<IPasswordHashingService, PasswordHashing>();
services.AddScoped<IAuthenticationService, AuthenticationService>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddSingleton<ITokenService, TokenService>();
services.AddDbContext<TdbContext>(options =>
    options.UseSqlServer(sqlserver));
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redis;
    options.InstanceName = LoadValues.LoadValue("RedisInstanceName", configuration) ?? "DefaultInstance";
});
services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    EndPoints = { $"{redis}" },
    Ssl = false,
    AbortOnConnectFail = false,
}));
services.AddSingleton<IDatabase>(sp =>
{
    var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
    return multiplexer.GetDatabase();
});
services.AddSingleton(
    LuaScript.Prepare(
        "local requests = redis.call('INCR', KEYS[1])\n\nif requests == 1 then\n    redis.call('EXPIRE', KEYS[1], ARGV[2])\nend\n\nif requests > tonumber(ARGV[1]) then\n    return 1\nelse\n    return 0\nend")
);
services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = LoadValues.LoadValue("ISSUER", configuration) ?? configuration["JwtConfig:Issuer"] ??
                throw new ConfigurationValueMissingException(nameof(TokenValidationParameters.ValidateIssuer)),
            ValidAudience = LoadValues.LoadValue("AUDIENCE", configuration) ?? configuration["JwtConfig:Audience"] ??
                throw new ConfigurationValueMissingException(nameof(TokenValidationParameters.ValidAudience)),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                LoadValues.LoadValue("KEY", configuration) ?? configuration["JwtConfig:KEY"] ??
                throw new ConfigurationValueMissingException(nameof(TokenValidationParameters.IssuerSigningKey)))),
        };
        options.SaveToken = true;
    }).AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = LoadValues.LoadEnv("CLIENT_ID") ?? configuration["OAuth:Google:ClientId"] ??
            throw new ConfigurationValueMissingException(nameof(googleOptions.ClientId));
        googleOptions.ClientSecret = LoadValues.LoadValue("CLIENT_SECRET", configuration) ??
                                     configuration["OAuth:Google:ClientSecret"] ??
                                     throw new ConfigurationValueMissingException(nameof(googleOptions.ClientSecret));
    });
;
services.AddAuthorization();
return services;
    }
}