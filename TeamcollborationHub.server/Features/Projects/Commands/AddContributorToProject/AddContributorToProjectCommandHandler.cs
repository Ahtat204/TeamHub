using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;

public class AddContributorToProjectCommandHandler(TdbContext db):IRequestHandler<AddContributorToProjectCommand,User?>
{
    public async Task<User?> Handle(AddContributorToProjectCommand request, CancellationToken cancellationToken)
    {
        var result =  db.Users.FirstOrDefault(u => u.Id == request.UserId)  ;
        if (result is null) throw new NotFoundException<User>();
        db.Users.Add(result);
       await db.SaveChangesAsync(cancellationToken);
        return result;
    }
}