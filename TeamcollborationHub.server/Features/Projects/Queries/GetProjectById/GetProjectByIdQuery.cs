using System.ComponentModel.DataAnnotations;
using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery([Required]int Id) : IRequest<Project>;