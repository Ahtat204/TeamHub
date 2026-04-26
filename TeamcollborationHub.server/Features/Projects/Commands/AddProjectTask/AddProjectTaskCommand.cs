using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddProjectTask;
/// <summary>
/// a command to create and add a Task to a Project
/// </summary>
/// <param name="ProjectId">the Project identifier</param>
/// <param name="task">the task object , of type "ProjectTask"</param>
public record AddProjectTaskCommand(int ProjectId, ProjectTask? task) : IRequest<Project>;