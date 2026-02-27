namespace TeamcollborationHub.server.Entities;

public class RefreshToken
{
    
    public Guid Id { get; init; }
    public string Token { get; init; }=string.Empty;
    public DateTime Expires { get; init; }
    public int UserId { get; init; }
    public User? User { get; init; }
    
}