using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddProjectComment;

public record AddProjectCommentCommand(int ProjectId, Comment? Comment):IRequest<Project>;