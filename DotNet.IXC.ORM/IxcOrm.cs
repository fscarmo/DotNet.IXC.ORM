using DotNet.IXC.ORM.Api;
using DotNet.IXC.ORM.Api.Records;
using DotNet.IXC.ORM.Enums;


namespace DotNet.IXC.ORM;


public class IxcOrm(string table) : RequestEmitter(table)
{
    private readonly List<Parameter> parameters = [];


    private Ordering ordering = Ordering.AscBy(table, "id");
    private Pagination pagination = Pagination.Default;
    private Parameter.Builder parameterBuilder = Parameter.NewBuilder(table);


    public IxcOrm WithPagination(int page, int rows)
    {
        return WithPagination(new Pagination(page, rows));
    }


    public IxcOrm WithPagination(Pagination pagination)
    {
        this.pagination = pagination;
        return this;
    }


    public IxcOrm OrderBy(string sortName, IxcOrmSort sortOrder)
    {
        ordering = (sortOrder == IxcOrmSort.Desc)
            ? Ordering.DescBy(Table, sortName)
            : Ordering.AscBy(Table, sortName);

        SetupQuery(GetQueryAsJsonString());

        return this;
    }


    public IxcOrm Where(string column)
    {
        parameterBuilder.WithType(column);
        return this;
    }


    public IxcOrm Like(object value)
    {
        parameterBuilder.WithOperator(IxcOrmOperator.Like);
        parameterBuilder.WithValue(value);

        parameters.Add(parameterBuilder.Build());
        parameterBuilder = Parameter.NewBuilder(Table);

        SetupQuery(GetQueryAsJsonString());

        return this;
    }


    public bool ValidateQuery(string expected)
    {
        string normalizedExpected = expected
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace(" ", "");
        string actual = GetQueryAsJsonString();
        return normalizedExpected.Equals(actual);
    }


    private string GetQueryAsJsonString()
    {
        string queryPropsAsJson = (
            $"""
            "qtype":"{Table}",
            "query":"",
            "oper":"",
            "page":"{pagination.Page}",
            "rp":"{pagination.Rows}",
            "sortname":"{ordering.SortName}",
            "sortorder":"{ordering.SortOrder.Value()}"
            """
        ).Replace("\r", "")
         .Replace("\n", "")
         .Replace(" ", "");

        string gridParamsContent = string.Join(",", parameters.Select(param => param.ToString()));
        string gridParamAsJson = $"\"grid_param\":\"[{gridParamsContent}]\"";

        return $"{{{queryPropsAsJson},{gridParamAsJson}}}";
    }
}
