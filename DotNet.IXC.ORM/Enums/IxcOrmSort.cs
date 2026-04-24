namespace DotNet.IXC.ORM.Enums;


public enum IxcOrmSort
{
    Asc,
    Desc
}


public static class IxcOrmSortExtensions
{
    public static string Value(this IxcOrmSort sort)
    {
        return sort switch
        {
            IxcOrmSort.Asc => "asc",
            IxcOrmSort.Desc => "desc",
            _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, null)
        };
    }
}
