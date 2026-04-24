using DotNet.IXC.ORM.Api;
using DotNet.IXC.ORM.Enums;


namespace DotNet.IXC.ORM;


public class IxcOrm(string table) : RequestEmitter(table)
{
    private readonly List<Parameter> parameters = [];


    private Parameter.Builder parameterBuilder = Parameter.NewBuilder(table);


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

        SetupQuery(GetQueryAsJson());

        return this;
    }


    public String GetQueryAsJson()
    {
        String jsonQueryProps = GetQueryPropsAsJsonString();
        String jsonGridParams = GetGridParamsAsJson();
        return $"{{{jsonQueryProps},{jsonGridParams}}}";
    }


    private string GetQueryPropsAsJsonString()
    {
        return $"""
        "qtype":"{Table}",
        "query":"",
        "oper":"",
        "page":"1",
        "rp":"20",
        "sortname":"{Table}.id",
        "sortorder":"asc"
        """;
    }


    private string GetGridParamsAsJson()
    {
        var content = string.Join(",", parameters.Select(param => param.ToString()));
        return $"\"grid_param\":\"[{content}]\"";
    }
}
