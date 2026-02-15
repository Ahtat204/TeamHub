using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Repositories.UserRepository;
/// <summary>
/// class Responsible for handling authentication-related database operations on the User Entity
/// </summary>
public class UserRepository:IUserRepository
{
    /// <summary>
    /// an instance of TDBContext to interact with the database
    /// </summary>
    private readonly TdbContext _context;
    /// <summary>
    /// a Constructor that injects an instance of TDBContext
    /// </summary>
    /// <param name="context"></param>
    public UserRepository(TdbContext context)
    {
        _context = context;
    }

    public UserRepository()
    {
        
    }
    /// <summary>
    ///  an asynchronous method responsible for creating a new user in the database
    /// </summary>
    /// <param name="user">a user Object to be inserted in database</param>
    /// <returns>return the created user , good for testing </returns>
    public async Task<User> CreateUser(User user)
    {
        _context.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
    /// <summary>
    /// an asynchronous method for searching a user by email in the database
    /// </summary>
    /// <param name="email">the email of the targeted <code>User</code>></param>
    /// <returns>returns the user associated with the email if found</returns>
    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u=>u.Email==email);
    }
    /// <summary>
    /// for fast lookup , a method for searching by an id
    /// </summary>
    /// <param name="id">the unique identifier of the <code>User</code>></param>
    /// <returns>returns the user if found</returns>
    public  async Task<User?> GetUserById(int id)
    {
        return await _context.FindAsync<User>(id);
    }
    /// <summary>
    /// method returns all the users in the database
    /// </summary>
    /// <returns></returns>
    public  IQueryable<User> GetAllUsers()
    {
        return _context.Set<User>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<User> deleteUser(int id)
    {
        var user =await  _context.FindAsync<User>(id);
        if (user == null) return user!;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return user;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<User> UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
}
