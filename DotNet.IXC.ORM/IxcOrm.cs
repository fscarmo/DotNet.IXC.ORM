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


    public bool ValidateQuery(string expected)
    {
        string normalizedExpected = expected
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace(" ", "");

        string actual = GetQueryAsJsonString();

        return normalizedExpected.Equals(actual);
    }


    public IxcOrm WithPagination(int page, int rows)
    {
        pagination = new Pagination(page, rows);
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
        parameterBuilder
            .WithOperator(IxcOrmOperator.Like)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    public IxcOrm Exactly(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.Equals)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    public IxcOrm Not(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.Not)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    public IxcOrm LessThan(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.LessThan)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    public IxcOrm LessThanOrEqual(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.LessThanEquals)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    public IxcOrm GreaterThan(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.GreaterThan)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    public IxcOrm GreaterThanOrEqual(object value)
    {
        parameterBuilder
            .WithOperator(IxcOrmOperator.GreaterThanEquals)
            .WithValue(value);

        PrepareQuery();

        return this;
    }


    private void PrepareQuery()
    {
        parameters.Add(parameterBuilder.Build());
        parameterBuilder = Parameter.NewBuilder(Table);
        SetupQuery(GetQueryAsJsonString());
    }


    protected override async Task<string> EmmitRequest(HttpMethod method, CancellationToken? cancellationToken = null)
    {
        string result = await base.EmmitRequest(method, cancellationToken);

        parameterBuilder = Parameter.NewBuilder(Table);
        parameters.Clear();
        SetupQuery(string.Empty);

        return result;
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
