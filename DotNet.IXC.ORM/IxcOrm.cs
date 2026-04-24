using DotNet.IXC.ORM.Api;
using DotNet.IXC.ORM.Api.Records;
using DotNet.IXC.ORM.Enums;


namespace DotNet.IXC.ORM;


public class IxcOrm(string table) : RequestEmitter(table)
{
    private readonly List<Parameter> parameters = [];


    private Parameter.Builder parameterBuilder = Parameter.NewBuilder(table);
    private Ordering ordering = Ordering.AscBy(table, "id");


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


    public IxcOrm OrderBy(string sortName, IxcOrmSort sortOrder)
    {
        ordering = (sortOrder == IxcOrmSort.Desc)
            ? Ordering.DescBy(Table, sortName)
            : Ordering.AscBy(Table, sortName);

        SetupQuery(GetQueryAsJsonString());

        return this;
    }


    public string GetQueryAsJsonString()
    {
        string queryPropsAsJson = (
            $"""
            "qtype":"{Table}",
            "query":"",
            "oper":"",
            "page":"1",
            "rp":"20",
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
