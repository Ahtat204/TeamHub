using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Mediator.Commands;
using TeamcollborationHub.server.Mediator.Handlers;

namespace TeamcollborationHub.server.Endpoints;

public static class CreateProject
{
    //TODO:don't use the mediator here, use the IProjectsService,even if it looks like unnecessary abstraction
    public static WebApplication MapPostCreateProject(this WebApplication app)
    {
     app.MapPost("/api/projects", async (CreateProjectCommand command, IMediator mediator) =>
     {
         var result = await mediator.Send(command);
         return Results.Created($"/api/projects/{result.Id}", result);
     }).WithTags("Projects").WithName("CreateProject");
     return app;
    }
}