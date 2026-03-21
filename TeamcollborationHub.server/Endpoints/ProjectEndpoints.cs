using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;
using TeamcollborationHub.server.Features.Projects.Commands.AddProjectTask;
using TeamcollborationHub.server.Features.Projects.Commands.CreateProject;
using TeamcollborationHub.server.Features.Projects.Commands.RemoveContributorFromProject;
using TeamcollborationHub.server.Features.Projects.Commands.RemoveProjectTask;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectContributors;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectTasks;
using TeamcollborationHub.server.Features.Projects.Queries.GetProjectById;
using TeamcollborationHub.server.Features.Projects.Queries.GetProjectTaskById;

namespace TeamcollborationHub.server.Endpoints;

public static class ProjectEndpoints
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        #region GetRequests

        app.MapGet("api/projects", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllProjectsQuery());
            return Results.Ok(result);
        }); //tested
        app.MapGet("api/projects/{id:int}", async ( int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProjectByIdQuery(id));
            return Results.Ok(result);
        });
        app.MapGet("api/projects/{id:int}/contributors", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllProjectContributorsQuery(id));
            return Results.Ok(result);
        });
        app.MapGet("api/{id:int}/task", async ([FromQuery] int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllProjectTasksQuery(id));
            return Results.Ok(result);
        }); //fetch all Task of a specific project
        app.MapGet("api/project/tasks/{id:int}", async ([FromQuery] int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProjectTaskByIdQuery(id));
            return Results.Ok(result);
        });
        app.MapGet("api/projects/contributors/{id:int}", async ([FromQuery] int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllProjectContributorsQuery(id));
            return Results.Ok(result);
        });

        #endregion
        #region PostRequests

        app.MapPost("api/projects", async (CreateProjectCommand projectCommand, IMediator mediator) =>
        {
            var result = await mediator.Send(projectCommand);
            return Results.Created($"api/projects/{result}", result);
        });
        app.MapPost("api/projects/contributors",
            async (AddContributorToProjectCommand addContributor, IMediator mediator) =>
            {
                var result = await mediator.Send(addContributor);
                return Results.Created($"api/projects/contributors{result}", result);
            });
        app.MapPost("api/projects/tasks", async (AddProjectTaskCommand addProject, IMediator mediator) =>
        {
            var result = await mediator.Send(addProject);
            return Results.Created($"api/projects/tasks/{result}", result);
        });

        #endregion
        #region DeleteRequests

        app.MapDelete("api/projects/{projectid:int}/contributor/{id:int}",
            async (IMediator mediator, int ID, int id) =>
            {
                await mediator.Send(new RemoveContributorFromProjectCommand(ID, id));
                return Results.NoContent();
            });
        app.MapDelete("api/projects/task/{id:int}", async (IMediator mediator, [FromQuery] int id) =>
        {
            await mediator.Send(new RemoveProjectTaskCommand(id));
            return Results.NoContent();
        });
        #endregion

        return app;
    }
}