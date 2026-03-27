using MediatR;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.SetProjectDeadline;

public class SetProjectDeadlineCommandHandler(TdbContext db):IRequestHandler<SetProjectDeadlineCommand,Project>
{
    public async Task<Project> Handle(SetProjectDeadlineCommand request, CancellationToken cancellationToken)
    {
        var project = db.Projects.SingleOrDefault(p => p.Id == request.ProjectId) ??
                      throw new NotFoundException<Project>();
        project.Deadline = request.Deadline;    
        await db.SaveChangesAsync(cancellationToken);
        return project;
    }
}