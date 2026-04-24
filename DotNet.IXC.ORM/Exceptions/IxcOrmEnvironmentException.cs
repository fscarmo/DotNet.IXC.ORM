namespace DotNet.IXC.ORM.Exceptions;


public class IxcOrmEnvironmentException(string environment)
    : IxcOrmException($"A variável \"{environment}\" não foi encontrada.")
{
}
