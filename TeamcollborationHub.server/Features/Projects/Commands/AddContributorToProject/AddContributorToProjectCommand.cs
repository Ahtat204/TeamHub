using MediatR;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Features.Projects.Commands.AddContributorToProject;

/// <summary>
/// 
/// </summary>
/// <param name="ProjectId"></param>
/// <param name="UserId"></param>
public record AddContributorToProjectCommand(int ProjectId, int UserId) : IRequest<Project>;