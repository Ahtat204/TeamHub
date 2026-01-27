using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using System.Text;
using TeamcollborationHub.server.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeamcollborationHub.server.Services.Caching;
using Microsoft.IdentityModel.Tokens;
using TeamcollborationHub.server.Exceptions;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Security;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TDBContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("sqlserverconnectionstring") ??
                         throw new InvalidOperationException(
                             "Connection string 'sqlserverconnectionstring' not found.")));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("RedisConnectionString");
    options.InstanceName = LoadValues.LoadValue("RedisInstanceName",configuration) ?? "DefaultInstance";
});
builder.Services.AddScoped<ICachingService, RedisCachingService>();
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
            ValidIssuer =  LoadValues.LoadValue("JwtConfig:Issuer",configuration) ,
            ValidAudience = LoadValues.LoadValue("JwtConfig:Audience", configuration),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(LoadValues.LoadValue("JwtConfig:KEY", configuration) ?? string.Empty)),
        };
        options.SaveToken = true;
    });
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
