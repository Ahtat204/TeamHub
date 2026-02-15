using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;

public record AddContributorToProjectCommand(int ProjectId, int UserId) : IRequest<User?>;