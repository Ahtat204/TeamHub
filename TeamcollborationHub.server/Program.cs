using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Services.Authentication;
using TeamcollborationHub.server.Services.Caching;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnectionString") ?? throw new InvalidOperationException("Connection string 'TeamcollborationHubContext' not found.")));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
    options.InstanceName = builder.Configuration.GetValue<string>("RedisInstanceName") ?? "DefaultInstance";
});
builder.Services.AddScoped<ICachingService, RedisCachingService>();
builder.Services.AddKeyedScoped<IAuthenticationService, AuthenticationService>("AuthenticationService");
builder.Services.AddSingleton<TDBContext>();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();