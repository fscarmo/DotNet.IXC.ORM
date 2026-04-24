namespace DotNet.IXC.ORM.Exceptions
{
    public class IxcOrmArgumentException(string paramName)
        : IxcOrmException($"O argumento \"{paramName}\" é inválido. O valor fornecido é nulo ou vazio.")
    {
    }
}
