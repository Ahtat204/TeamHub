using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Mediator.Queries;

public record GetAllProjectsQuery() : IRequest<List<Project>>; 
public record GetAllProjectContributorsQuery(int id) : IRequest<List<User>>;
public record GetAllProjectTasksQuery(int ProjectId) : IRequest<List<ProjectTask>>;
public record GetProjectByIdQuery(int Id) : IRequest<Project>;
public record GetProjectContributorsByIdQuery(int ProjectId) : IRequest<List<User>>;
public record GetProjectTaskByIdQuery(int Id) : IRequest<ProjectTask>;
