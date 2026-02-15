using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetProjectTaskById;

/// <summary>
/// 
/// </summary>
/// <param name="Id">this the Task id not the project Id</param>
public record GetProjectTaskByIdQuery(int Id) : IRequest<ProjectTask>;
