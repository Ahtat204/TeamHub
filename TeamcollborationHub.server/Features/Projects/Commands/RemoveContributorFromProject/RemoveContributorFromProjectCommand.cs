using MediatR;
using TeamcollborationHub.server.Entities;
namespace TeamcollborationHub.server.Features.Projects.Commands.RemoveContributorFromProject;

public record RemoveContributorFromProjectCommand(int ProjectId, int UserId) : IRequest<Project>;