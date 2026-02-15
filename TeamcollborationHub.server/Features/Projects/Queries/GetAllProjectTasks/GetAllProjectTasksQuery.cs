using System.ComponentModel.DataAnnotations;
using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectTasks;

public record GetAllProjectTasksQuery([Required]int ProjectId) : IRequest<IEnumerable<ProjectTask>?>;