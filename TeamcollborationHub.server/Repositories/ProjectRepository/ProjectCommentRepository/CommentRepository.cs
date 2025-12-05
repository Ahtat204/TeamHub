using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Repositories.ProjectRepository.ProjectCommentRepository;

public class CommentRepository: IProjectCommentRepository
{
    public IQueryable<Comment?> GetAllProjectComments(int projectId)
    {
        throw new NotImplementedException();
    }
}