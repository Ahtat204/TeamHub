using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Repositories.UserRepository;

public interface IUserRepository
{
    public Task<User> CreateUser(User user);
    public Task<User?> GetUserByEmail(string email);
    public Task<User?> GetUserById(int id);
    public IQueryable<User> GetAllUsers();
    public Task<User> deleteUser(int id);
    public Task<User> UpdateUser(User user);

}