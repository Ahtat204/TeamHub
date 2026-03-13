namespace TeamcollborationHub.server.Exceptions;

public class AlreadyExistsException<T>(T email) : Exception($"the email {email} is already in use");