using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Repositories.UserRepository;

public interface IUserRepository
{
    public Task<User?> CreateUser(User user);
    public Task<User?> GetUserByEmail(string email);
    public Task<User?> GetUserById(int id);
    public IQueryable<User> GetAllUsers();
    public Task<User> DeleteUser(int id);
    public Task<User> DeleteUser(string email);
    public Task<User> UpdateUser(User user);
    public Task<string?> SaveRefreshToken(RefreshToken refreshToken);

}