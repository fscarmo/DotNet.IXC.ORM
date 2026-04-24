using DotNet.IXC.ORM.Enums;


namespace DotNet.IXC.ORM.Api.Records;


public record Ordering(string SortName, IxcOrmSort SortOrder)
{
    public static Ordering AscBy(string table, string column)
        => new($"{table}.{column}", IxcOrmSort.Asc);

    public static Ordering DescBy(string table, string column)
        => new($"{table}.{column}", IxcOrmSort.Desc);
}
