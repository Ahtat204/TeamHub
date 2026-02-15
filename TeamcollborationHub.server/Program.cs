using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeamcollborationHub.server.Services.Caching;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamcollborationHub.server.Features.Projects.Commands.CreateProject;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectContributors;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectsQuery;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectTasks;
using TeamcollborationHub.server.Features.Projects.Queries.GetProjectById;
using TeamcollborationHub.server.Features.Projects.Queries.GetProjectTaskById;
using TeamcollborationHub.server.Repositories.UserRepository;
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
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TdbContext>(options =>
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
builder.Services.AddScoped<IUserRepository,UserRepository>();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#region GetRequests
app.MapGet("api/projects", async (GetAllProjectsQuery get, IMediator mediator) =>
{
    var result = await mediator.Send(get);
    return  Results.Ok(result);
});
app.MapGet("api/projects/{id:int}", async ( [FromQuery]int id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetProjectByIdQuery(id));
    return  Results.Ok(result);
});
app.MapGet("api/projects/{id:int}/contributors", async ([FromQuery] int id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAllProjectContributorsQuery(id));
    return  Results.Ok(result);
});
app.MapGet("api/{id:int}/task", async ([FromQuery] int id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAllProjectTasksQuery(id));
    return  Results.Ok(result);
}); //fetch all Task of a specific project
app.MapGet("api/project/tasks/{id:int}", async ([FromQuery] int id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetProjectTaskByIdQuery(id));
    return  Results.Ok(result);
});
app.MapGet("api/project/contributor/{id:int}", async ([FromQuery] int id, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAllProjectContributorsQuery(id));
    return  Results.Ok(result);
});
#endregion
#region PostRequests

app.MapPost("api/projects/", async (CreateProjectCommand projectCommand, IMediator mediator) =>
{
    var result = await mediator.Send(projectCommand);
    return  Results.Ok(result);
});
#endregion
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();