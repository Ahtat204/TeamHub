namespace TeamcollborationHub.server.Exceptions;

public class ValueNotFoundException : Exception
{
   private readonly string secretName;
    public ValueNotFoundException( string secretName) : base(secretName+"was not found")
    {
        this.secretName = secretName;
    }
}
