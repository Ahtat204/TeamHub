using MediatR;

namespace TeamcollborationHub.server.Features.Projects.Commands.RemoveContributorFromProject;

public record RemoveContributorFromProject(int ProjectId, int UserId) : IRequest<bool>;