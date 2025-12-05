using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeamcollborationHub.server.Services.Caching;
using Microsoft.IdentityModel.Tokens;
using dotenv.net;
using System.Text;
using TeamcollborationHub.server.Repositories;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Security;

var builder = WebApplication.CreateBuilder(args);
// DotEnv.Load();
// Add services to the container.
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnectionString") ??
                         throw new InvalidOperationException(
                             "Connection string 'SQLServerConnectionString' not found.")));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
    options.InstanceName = builder.Configuration.GetValue<string>("RedisInstanceName") ?? "DefaultInstance";
});
builder.Services.AddScoped<ICachingService, RedisCachingService>();
builder.Services.AddSingleton<IPasswordHashingService, PasswordHashing>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
            ValidAudience = builder.Configuration["JwtConfig:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!))
        };
        options.SaveToken = true;
    });
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();