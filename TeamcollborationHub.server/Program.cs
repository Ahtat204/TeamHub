using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using dotenv.net;
using TeamcollborationHub.server.Helpers;
using TeamcollborationHub.server.Services.Caching;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using TeamcollborationHub.server.Endpoints;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Middlewares;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using TeamcollborationHub.server.Services.Security;



DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration
    .AddEnvironmentVariables(configureSource: source => { source.Prefix = ".env"; }).AddUserSecrets<Program>().Build();
string sqlserver = LoadValues.LoadValue("sqlserverconnectionstring", configuration) ??
                configuration.GetConnectionString("sqlserverconnectionstring") ??
                throw new InvalidOperationException("SQL Server Connection string wasn't not found.");
string redis=LoadValues.LoadValue("RedisConnectionString",configuration)??configuration.GetConnectionString("RedisConnectionString") ?? throw new InvalidOperationException("Redis Connection string  wasn't found .");
#region DependencyInjection
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TdbContext>(options =>
    options.UseSqlServer( sqlserver));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redis;
    options.InstanceName = LoadValues.LoadValue("RedisInstanceName",configuration) ?? "DefaultInstance";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    EndPoints = { $"{redis}" },
    Ssl = false,
    AbortOnConnectFail = false,
}));
builder.Services.AddSingleton<IDatabase>(sp =>
{
    var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
    return multiplexer.GetDatabase();
});
builder.Services.AddSingleton(
    LuaScript.Prepare(
        "local requests = redis.call('INCR', KEYS[1])\n\nif requests == 1 then\n    redis.call('EXPIRE', KEYS[1], ARGV[2])\nend\n\nif requests > tonumber(ARGV[1]) then\n    return 1\nelse\n    return 0\nend")
);
builder.Services.AddScoped<ICachingService<Project, string>, RedisCachingService>();
builder.Services.AddScoped<ICachingService<RefreshToken, string>, RefreshTokenCachingService>();
builder.Services.AddSingleton<IPasswordHashingService, PasswordHashing>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddAuthentication(opt =>
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
                throw new ValueNotFoundException(nameof(TokenValidationParameters.ValidateIssuer)),
            ValidAudience = LoadValues.LoadValue("AUDIENCE", configuration) ?? configuration["JwtConfig:Audience"] ??
                throw new ValueNotFoundException(nameof(TokenValidationParameters.ValidAudience)),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                LoadValues.LoadValue("KEY", configuration) ?? configuration["JwtConfig:KEY"] ??
                throw new ValueNotFoundException(nameof(TokenValidationParameters.IssuerSigningKey)))),
        };
        options.SaveToken = true;
    }).AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = LoadValues.LoadEnv("CLIENT_ID") ?? configuration["OAuth:Google:ClientId"] ??
            throw new ValueNotFoundException(nameof(googleOptions.ClientId));
        googleOptions.ClientSecret = LoadValues.LoadValue("CLIENT_SECRET", configuration) ??
                                     configuration["OAuth:Google:ClientSecret"] ??
                                     throw new ValueNotFoundException(nameof(googleOptions.ClientSecret));
    });
;
builder.Services.AddAuthorization();

#endregion

var app = builder.Build();
app.MapEndpoints();// TODO:Don't forget this ,this top priority ,without this line (without "//") ,CQRS is just a folder structure 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Middlewares

app.UseExceptionHandler();
if (app.Environment.IsTesting())
{
    app.Use(async (context, next) =>
    {
        context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("127.0.0.1");
        await next();
    });
}
app.UseIpBasedRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

#endregion

app.Run();

public partial class Program
{
} // added to solve Can't find <'TeamcollaborationHub\TeamCollaborationHub.server.IntegrationTest\bin\Debug\net8.0\testhost.deps.json'> problem 