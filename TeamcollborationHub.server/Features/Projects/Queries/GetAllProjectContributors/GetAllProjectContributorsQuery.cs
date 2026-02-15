using System.ComponentModel.DataAnnotations;
using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectContributors;

public record GetAllProjectContributorsQuery([Required]int id) : IRequest<List<User>>;