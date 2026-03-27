using MediatR;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Exceptions;

namespace TeamcollborationHub.server.Features.Projects.Commands.SetProjectStartDate;
//TODO:Add Database Constraint to Deadline and StartDate , Deadline should be bigger than StartDate,in case if the date slipped into the database
public class SetProjectStartDateCommandHandler(TdbContext db):IRequestHandler<SetProjectStartDateCommand,Project>
{
    public async Task<Project> Handle(SetProjectStartDateCommand request, CancellationToken cancellationToken)
    {
        if (request.StartDate < DateTime.Now) throw new InvalidDateException("looks like you're a fan of Einstein");
        var project = await db.Projects.SingleOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken: cancellationToken) ?? throw new NotFoundException<Project>();
        if(project.Deadline is not null && request.StartDate>=project.Deadline) throw new InvalidDateException("try again");
        project.Deadline = request.StartDate;
        await db.SaveChangesAsync(cancellationToken);
        return project;
    }
}