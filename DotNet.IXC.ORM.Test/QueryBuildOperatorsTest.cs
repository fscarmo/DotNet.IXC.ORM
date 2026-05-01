namespace DotNet.IXC.ORM.Test;


public class QueryBuildOperatorsTest
{
    [Fact]
    public void QueryLikeOperator()
    {
        using var host = Utils.MockedHostBuilder().Build();

        var ixcOrm = new IxcOrm("test_table")
            .Where("name")
            .Like("John");

        string expected = """
        {
            "qtype":"test_table",
            "query":"",
            "oper":"",
            "page":"1",
            "rp":"20",
            "sortname":"test_table.id",
            "sortorder":"asc",
            "grid_param":"[{\"TB\":\"test_table.name\",\"OP\":\"L\",\"P\":\"John\"}]"
        }
        """;

        Assert.True(ixcOrm.ValidateQuery(expected));
    }


    [Fact]
    public void QueryExactlyOperator()
    {
        using var host = Utils.MockedHostBuilder().Build();

        var ixcOrm = new IxcOrm("test_table")
            .Where("name")
            .Exactly("John");

        string expected = """
        {
            "qtype":"test_table",
            "query":"",
            "oper":"",
            "page":"1",
            "rp":"20",
            "sortname":"test_table.id",
            "sortorder":"asc",
            "grid_param":"[{\"TB\":\"test_table.name\",\"OP\":\"=\",\"P\":\"John\"}]"
        }
        """;

        Assert.True(ixcOrm.ValidateQuery(expected));
    }


    [Fact]
    public void QueryNotOperator()
    {
        using var host = Utils.MockedHostBuilder().Build();

        var ixcOrm = new IxcOrm("test_table")
            .Where("name")
            .Not("John");

        string expected = """
        {
            "qtype":"test_table",
            "query":"",
            "oper":"",
            "page":"1",
            "rp":"20",
            "sortname":"test_table.id",
            "sortorder":"asc",
            "grid_param":"[{\"TB\":\"test_table.name\",\"OP\":\"!=\",\"P\":\"John\"}]"
        }
        """;

        Assert.True(ixcOrm.ValidateQuery(expected));
    }


    [Fact]
    public void QueryLessThanOperator()
    {
        using var host = Utils.MockedHostBuilder().Build();

        var ixcOrm = new IxcOrm("test_table")
            .Where("age")
            .LessThan(30);

        string expected = """
        {
            "qtype":"test_table",
            "query":"",
            "oper":"",
            "page":"1",
            "rp":"20",
            "sortname":"test_table.id",
            "sortorder":"asc",
            "grid_param":"[{\"TB\":\"test_table.age\",\"OP\":\"<\",\"P\":\"30\"}]"
        }
        """;

        Assert.True(ixcOrm.ValidateQuery(expected));
    }


    [Fact]
    public void QueryLessThanOrEqualOperator()
    {
        using var host = Utils.MockedHostBuilder().Build();

        var ixcOrm = new IxcOrm("test_table")
            .Where("age")
            .LessThanOrEqual(30);

        string expected = """
        {
            "qtype":"test_table",
            "query":"",
            "oper":"",
            "page":"1",
            "rp":"20",
            "sortname":"test_table.id",
            "sortorder":"asc",
            "grid_param":"[{\"TB\":\"test_table.age\",\"OP\":\"<=\",\"P\":\"30\"}]"
        }
        """;

        Assert.True(ixcOrm.ValidateQuery(expected));
    }


    [Fact]
    public void QueryGreaterThanOperator()
    {
        using var host = Utils.MockedHostBuilder().Build();

        var ixcOrm = new IxcOrm("test_table")
            .Where("age")
            .GreaterThan(18);

        string expected = """
        {
            "qtype":"test_table",
            "query":"",
            "oper":"",
            "page":"1",
            "rp":"20",
            "sortname":"test_table.id",
            "sortorder":"asc",
            "grid_param":"[{\"TB\":\"test_table.age\",\"OP\":\">\",\"P\":\"18\"}]"
        }
        """;

        Assert.True(ixcOrm.ValidateQuery(expected));
    }


    [Fact]
    public void QueryGreaterThanOrEqualOperator()
    {
        using var host = Utils.MockedHostBuilder().Build();

        var ixcOrm = new IxcOrm("test_table")
            .Where("age")
            .GreaterThanOrEqual(18);

        string expected = """
        {
            "qtype":"test_table",
            "query":"",
            "oper":"",
            "page":"1",
            "rp":"20",
            "sortname":"test_table.id",
            "sortorder":"asc",
            "grid_param":"[{\"TB\":\"test_table.age\",\"OP\":\">=\",\"P\":\"18\"}]"
        }
        """;

        Assert.True(ixcOrm.ValidateQuery(expected));
    }
}
