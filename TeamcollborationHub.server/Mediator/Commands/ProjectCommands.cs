using System.ComponentModel.DataAnnotations;
using MediatR;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Enums;

namespace TeamcollborationHub.server.Mediator.Commands;


public record CreateProjectCommand(
       [Required] string Name,
       string Description,
       IEnumerable<User>? Contributors,
       ProjectStatus? ProjectStatus,
       DateTime? Deadline) : IRequest<Project>;

public record AddContributorToProjectCommand(int ProjectId, int UserId) : IRequest<User?>;
public record RemoveContributorFromProject(int ProjectId, int UserId) : IRequest<bool>;
public record AddProjectTask(int ProjectId, ProjectTask task) : IRequest<ProjectTask>;
public record RemoveProjectTask(int ProjectTaskId) : IRequest<bool>;
public record SetProjectStartDate(int ProjectId, DateTime StartDate) : IRequest<bool>;
public record SetProjectDeadline(int ProjectId, DateTime Deadline) : IRequest<DateTime>;