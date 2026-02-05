using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using System.Text;
using dotenv.net;
using TeamcollborationHub.server.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeamcollborationHub.server.Services.Caching;
using Microsoft.IdentityModel.Tokens;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Security;


DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration.AddEnvironmentVariables(configureSource: source => { source.Prefix = ".env";} ).AddUserSecrets<Program>().Build();
string clientId = configuration["OAuth:Google:ClientId"] ?? "not found";
Console.WriteLine(clientId);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TdbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("sqlserverconnectionstring") ??
                         throw new InvalidOperationException(
                             "Connection string 'sqlserverconnectionstring' not found.")));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("RedisConnectionString");
    options.InstanceName = LoadValues.LoadValue("RedisInstanceName",configuration) ?? "DefaultInstance";
});
builder.Services.AddScoped<ICachingService<Project,string>, RedisCachingService>();
builder.Services.AddScoped<ICachingService<RefreshToken, string>, RefreshTokenCachingService>();
builder.Services.AddSingleton<IPasswordHashingService, PasswordHashing>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserRepository,UserRepository>();
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
            ValidIssuer =  LoadValues.LoadValue("ISSUER",configuration) ?? configuration["JwtConfig:Issuer"] ?? throw new ValueNotFoundException(nameof(TokenValidationParameters.ValidateIssuer)),
            ValidAudience = LoadValues.LoadValue("AUDIENCE", configuration) ?? configuration["JwtConfig:Audience"] ?? throw new ValueNotFoundException(nameof(TokenValidationParameters.ValidAudience)),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(LoadValues.LoadValue("KEY", configuration) ?? configuration["JwtConfig:KEY"] ?? throw new ValueNotFoundException(nameof(TokenValidationParameters.IssuerSigningKey)))),
        };
        options.SaveToken = true;
    }).
    AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = LoadValues.LoadEnv("CLIENT_ID") ?? configuration["OAuth:Google:ClientId"] ??throw new ValueNotFoundException(nameof(googleOptions.ClientId));
        googleOptions.ClientSecret = LoadValues.LoadValue("CLIENT_SECRET",configuration) ?? configuration["OAuth:Google:ClientSecret"] ??throw new ValueNotFoundException(nameof(googleOptions.ClientSecret));
    }); ;
builder.Services.AddAuthorization();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
public partial class Program { } // added to solve Can't find <'TeamcollaborationHub\TeamCollaborationHub.server.IntegrationTest\bin\Debug\net8.0\testhost.deps.json'> problem 
