namespace TeamcollborationHub.server.Exceptions;

public class ValueNotFoundException(string secretName) : Exception(secretName + " \t was not found")
{
   private readonly string _secretName = secretName;
}
