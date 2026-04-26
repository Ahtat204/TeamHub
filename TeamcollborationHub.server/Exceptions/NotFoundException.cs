namespace TeamcollborationHub.server.Exceptions;

public class NotFoundException<T> : Exception
{
    public T? ConcreteEntity { get; }

    public NotFoundException()
        : base($"{typeof(T)} not found")
    { }

    public NotFoundException(string message)
        : base(message)
    { }

    public NotFoundException(T concreteEntity, string? message = null)
        : base(message ?? $"{typeof(T)} not found")
    {
        ConcreteEntity = concreteEntity;
    }
}