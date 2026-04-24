namespace DotNet.IXC.ORM.Exceptions;


public class IxcOrmRequestException(string message, Exception exeption) : IxcOrmException(message)
{
    public Exception Exeption { get; } = exeption;
}
