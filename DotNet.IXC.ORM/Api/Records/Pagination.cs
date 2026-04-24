namespace DotNet.IXC.ORM.Api.Records;


public record Pagination(int Page, int Rows)
{
    public static Pagination Default => new(1, 20);
}
