using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddProjectComment;
using MediatR;

public class AddProjectCommentCommandHandler(TdbContext db):IRequestHandler<AddProjectCommentCommand, Project>
{
    public async Task<Project> Handle(AddProjectCommentCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request?.Comment);
       var project=await db.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken: cancellationToken)??throw new NotFoundException<Project>("Project not found");
       request.Comment.projectId = project.Id;
       request.Comment.Project = project;
       await db.Comments.AddAsync(request.Comment, cancellationToken);
       await db.SaveChangesAsync(cancellationToken);
       return project;
    }
}