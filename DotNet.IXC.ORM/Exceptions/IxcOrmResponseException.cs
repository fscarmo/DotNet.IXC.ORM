namespace DotNet.IXC.ORM.Exceptions;


public class IxcOrmResponseException(string message, Exception exeption) : IxcOrmException(message)
{
    public Exception Exeption { get; } = exeption;
}
