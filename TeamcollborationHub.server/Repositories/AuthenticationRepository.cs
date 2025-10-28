using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamcollborationHub.server.Configuration;
using TeamcollborationHub.server.Entities;

namespace TeamcollborationHub.server.Repositories;
/// <summary>
/// class Responsible for handling authentication-related database operations on the User Entity
/// </summary>
public class AuthenticationRepository
{
    /// <summary>
    /// an instance of TDBContext to interact with the database
    /// </summary>
    private readonly TDBContext _context;
    /// <summary>
    /// a Constructor that injects an instance of TDBContext
    /// </summary>
    /// <param name="context"></param>
    public AuthenticationRepository(TDBContext context)
    {
        _context = context;
    }
    /// <summary>
    ///  an asynchronous method responsible for creating a new user in the database
    /// </summary>
    /// <param name="user">a user Object to be inserted in database</param>
    /// <returns>return the created user , good for testing </returns>
    public User CreateUser(User user)
    {
        _context.Add(user);
        _context.SaveChanges();
        return user;
    }
    /// <summary>
    /// an asynchronous method for searching a user by email in the database
    /// </summary>
    /// <param name="email">the email of the targeted <code>User</code>></param>
    /// <returns>rturns the user assosiated with the email if found</returns>
    public User? GetUserByEmail(string email)
    {
        return _context.Users.SingleOrDefault(u=>u.Email==email);
    }
    /// <summary>
    /// for fast lookup , a method for searching by an id
    /// </summary>
    /// <param name="id">the unique identifier of the <code>User</code>></param>
    /// <returns>returns the user if found</returns>
    public User? GetUserById(int id)
    {
        return  _context.Find<User>(id);
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
    public User deleteUser(int id)
    {
        var user = _context.Find<User>(id);
        if (user != null)
        {
            _context.Users.Remove(user);
             _context.SaveChanges();
        }
        return user!;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public User UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
        return user;
    }
}
