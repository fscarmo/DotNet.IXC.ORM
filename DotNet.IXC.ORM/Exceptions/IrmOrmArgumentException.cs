namespace DotNet.IXC.ORM.Exceptions
{
    public class IxcOrmArgumentException(string paramName)
        : IxcOrmExeption($"O argumento \"{paramName}\" é inválido. O valor fornecido é nulo ou vazio.")
    {
    }
}
