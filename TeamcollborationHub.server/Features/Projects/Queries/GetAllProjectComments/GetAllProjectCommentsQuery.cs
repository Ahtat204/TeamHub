using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Queries.GetAllProjectComments;

public record GetAllProjectCommentsQuery(int ProjectId):IRequest<IEnumerable<Comment>>;