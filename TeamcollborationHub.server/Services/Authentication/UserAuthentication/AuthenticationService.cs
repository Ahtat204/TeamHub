using TeamcollborationHub.server.Entities;
using TeamcollborationHub.server.Entities.Dto;
using TeamcollborationHub.server.Repositories;
using TeamcollborationHub.server.Services.Security;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using TeamcollborationHub.server.Services.Authentication.Jwt;
namespace TeamcollborationHub.server.Services.Authentication.UserAuthentication;

public class AuthenticationService(IConfiguration configuration,IPasswordHashingService passwordHashingService,AuthenticationRepository authenticationRepository,IJwtService jwtservice) : IAuthenticationService
{
    public User? createUser()
    {
        throw new NotImplementedException();
    }

    public User? deleteUser(int id)
    {
        throw new NotImplementedException();
    }

    public AuthenticationResponse? AuthenticateUser(UserRequestDto UserRequest)
    {
        var User=authenticationRepository.GetUserByEmail(UserRequest.Email);
        if(User is null) throw new Exception("User not found");

        bool verified= passwordHashingService.VerifyPassword(UserRequest.Password, User.Password);

        if(!verified) throw new Exception("Invalid credentials password");

        var accesstoken=jwtservice.GenerateTokenResponse(User,out var tokenExpiryDate);
        return new AuthenticationResponse (
          email:UserRequest.Email,AccessToken:accesstoken, ExpiryDate: (int)tokenExpiryDate.Subtract(DateTime.UtcNow).TotalSeconds);
    }

    public User? updateUser(User UserRequest)
    {
        throw new NotImplementedException();
    }
}
