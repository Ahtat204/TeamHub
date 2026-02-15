using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;

public class AddContributorToProjectCommandHandler(TdbContext db):IRequestHandler<AddContributorToProjectCommand,User?>
{
    public Task<User?> Handle(AddContributorToProjectCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}