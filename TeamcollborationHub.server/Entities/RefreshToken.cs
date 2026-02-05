namespace TeamcollborationHub.server.Entities;

public class RefreshToken
{
    
    public Guid Id { get; set; }
    public string? Token { get; set; }
    public DateTime Expires { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    
}