namespace DotNet.IXC.ORM.Exceptions;


public class IxcOrmEnvironmentException(string environment)
    : IxcOrmExeption($"A variável \"{environment}\" não foi encontrada.")
{
}
