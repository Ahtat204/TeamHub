using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeamcollborationHub.server.Services.Caching;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TeamcollborationHub.server.Repositories.UserRepository;
using TeamcollborationHub.server.Services.Authentication.UserAuthentication;
using TeamcollborationHub.server.Services.Authentication.Jwt;
using TeamcollborationHub.server.Services.Security;
using StackExchange.Redis;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Middlewares;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnectionString") ??
                         throw new InvalidOperationException(
                             "Connection string 'SQLServerConnectionString' not found.")));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddSingleton<IDatabase>(sp =>
{
    var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
    return multiplexer.GetDatabase(); // cheap pass-through
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
    options.InstanceName = builder.Configuration.GetValue<string>("RedisInstanceName") ?? "DefaultInstance";
});
builder.Services.AddScoped<ICachingService<Project>, RedisCachingService>();
var script = """
    local current = redis.call('incr', KEYS[1])
    if tonumber(current) == 1 then
        redis.call('expire', KEYS[1], ARGV[2])
    end
    if tonumber(current) > tonumber(ARGV[1]) then
        return 0
    else
        return 1
    end
    """.Trim();
// this script is not yet tested but it should work as follows: when a request comes in, the script will increment the request count for the IP address in Redis. If the current count is 1, it means this is the first request from this IP address, so we set an expiration time for the key. If the current count exceeds the maximum allowed requests, we return 0 to indicate that the request should be blocked. Otherwise, we return 1 to indicate that the request is allowed.
builder.Services.AddSingleton(
    LuaScript.Prepare("local current = redis.call('incr', KEYS[1])\nif tonumber(current) == 1 then\n    redis.call('expire', KEYS[1], ARGV[2])\nend\nif tonumber(current) > tonumber(ARGV[1]) then\n    return 0\nelse\n    return 1\nend")
);

builder.Services.AddSingleton<IPasswordHashingService, PasswordHashing>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationRepository,AuthenticationRepository>();
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
            ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
            ValidAudience = builder.Configuration["JwtConfig:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!))
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

app.UseIpBasedRateLimiter();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();