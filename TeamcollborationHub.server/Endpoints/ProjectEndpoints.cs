using MediatR;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;
using TeamcollborationHub.server.Features.Projects.Commands.AddProjectComment;
using TeamcollborationHub.server.Features.Projects.Commands.AddProjectTask;
using TeamcollborationHub.server.Features.Projects.Commands.CreateProject;
using TeamcollborationHub.server.Features.Projects.Commands.RemoveContributorFromProject;
using TeamcollborationHub.server.Features.Projects.Commands.RemoveProjectTask;
using TeamcollborationHub.server.Features.Projects.Commands.SetProjectDeadline;
using TeamcollborationHub.server.Features.Projects.Commands.SetProjectStartDate;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectContributors;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjects;
using TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectTasks;
using TeamcollborationHub.server.Features.Projects.Queries.GetProjectById;
using TeamcollborationHub.server.Features.Projects.Queries.GetProjectTaskById;
using TeamcollborationHub.server.Services.Caching;

namespace TeamcollborationHub.server.Endpoints;

public static class ProjectEndpoints
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        #region GetRequests

        //tested
        app.MapGet("api/projects", async (IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllProjectsQuery());
            return Results.Ok(result);
        }).RequireAuthorization();
        //tested
        app.MapGet("api/projects/{id:int}",
            async (int id, IMediator mediator, ICachingService<Project, string> cachingService) =>
            {
                Project result = cachingService.GetProjectFromCache(id.ToString()) ??
                                 await mediator.Send(new GetProjectByIdQuery(id));
                return Results.Ok(result);
            }).RequireAuthorization();
        //tested
        app.MapGet("api/projects/{id:int}/contributors", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllProjectContributorsQuery(id));
            return Results.Ok(result);
        }).RequireAuthorization();
        app.MapGet("api/projects/{id:int}/tasks", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllProjectTasksQuery(id));
            return Results.Ok(result);
        }).RequireAuthorization(); //tested
        app.MapGet("api/project/tasks/{id:int}", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetProjectTaskByIdQuery(id));
            return Results.Ok(result);
        }).RequireAuthorization(); //tested
        app.MapGet("api/projects/contributors/{id:int}", async (int id, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAllProjectContributorsQuery(id));
            return Results.Ok(result);
        }).RequireAuthorization(); //tested

        #endregion

        #region PostRequests

        
        app.MapPost("api/projects", async (CreateProjectCommand projectCommand, IMediator mediator,
            ICachingService<Project, string> cachingService) =>
        {
            var result = await mediator.Send(projectCommand);
            cachingService.SetProjectInCache(result.Id.ToString(), result);
            return Results.Created($"api/projects/{result}", result);
        }).RequireAuthorization();
        //I'm aware that this shouldn't a Post request  
        app.MapPost("api/projects/contributors",
            async (AddContributorToProjectCommand addContributor, IMediator mediator,
                ICachingService<Project, string> cachingService) =>
            {
                var result = await mediator.Send(addContributor);
                cachingService.SetProjectInCache(result.Id.ToString(), result);
                return Results.Created($"api/projects/contributors{result}", result);
            }).RequireAuthorization();
        // but this is a post request of course
        app.MapPost("api/projects/tasks", async (AddProjectTaskCommand addProject, IMediator mediator,
            ICachingService<Project, string> cachingService) =>
        {
            var result = await mediator.Send(addProject);
            cachingService.SetProjectInCache(result.Id.ToString(), result);
            return Results.Created($"api/projects/tasks/{result}", result);
        }).RequireAuthorization();

        app.MapPost("api/projects/{id:int}/comments",
            async (IMediator mediator, AddProjectCommentCommand commentCommand) =>
            {
                var result = await mediator.Send(commentCommand);
                return Results.Created($"api/projects/comments/{result}", result);
            });
        #endregion

        #region DeleteRequests

        app.MapDelete("api/projects/{projectid:int}/contributors/{id:int}",
            async (IMediator mediator, int projectid, int id) =>
            {
                await mediator.Send(new RemoveContributorFromProjectCommand(projectid, id));
                return Results.NoContent();
            }).RequireAuthorization();
        app.MapDelete("api/projects/{projectId:int}/tasks/{id:int}",
            async (IMediator mediator, int projectId, int id) =>
            {
                await mediator.Send(new RemoveProjectTaskCommand(projectId, id));
                return Results.NoContent();
            }).RequireAuthorization();

        #endregion

        #region UpdateRequests

        app.MapPut("api/projects/{id:int}", async (int id, IMediator mediator, DateTime deadline) =>
        {
            var result = await mediator.Send(new SetProjectDeadlineCommand(id,deadline));
            return Results.Ok(result);
        });
        
        #endregion
        return app;
    }
}