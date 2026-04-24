using DotNet.IXC.ORM.Enums;


namespace DotNet.IXC.ORM.Test;


public class QueryBuildOptionsTest
{
    [Fact]
    public void QueryWithPagination()
    {
        Utils.BuildHost();

        var ixcOrm = new IxcOrm("test_table")
            .WithPagination(5, 100)
            .Where("name").Like("John");

        string expected = """
        {
            "qtype":"test_table",
            "query":"",
            "oper":"",
            "page":"5",
            "rp":"100",
            "sortname":"test_table.id",
            "sortorder":"asc",
            "grid_param":"[{\"TB\":\"test_table.name\",\"OP\":\"L\",\"P\":\"John\"}]"
        }
        """;

        Assert.True(ixcOrm.ValidateQuery(expected));
    }


    [Fact]
    public void QueryWithSortAsc()
    {
        Utils.BuildHost();

        var ixcOrm = new IxcOrm("test_table")
            .OrderBy("age", IxcOrmSort.Asc)
            .Where("name").Like("John");

        string expected = """
        {
            "qtype":"test_table",
            "query":"",
            "oper":"",
            "page":"1",
            "rp":"20",
            "sortname":"test_table.age",
            "sortorder":"asc",
            "grid_param":"[{\"TB\":\"test_table.name\",\"OP\":\"L\",\"P\":\"John\"}]"
        }
        """;

        Assert.True(ixcOrm.ValidateQuery(expected));
    }


    [Fact]
    public void QueryWithSortDesc()
    {
        Utils.BuildHost();

        var ixcOrm = new IxcOrm("test_table")
            .OrderBy("age", IxcOrmSort.Desc)
            .Where("name").Like("John");

        string expected = """
        {
            "qtype":"test_table",
            "query":"",
            "oper":"",
            "page":"1",
            "rp":"20",
            "sortname":"test_table.age",
            "sortorder":"desc",
            "grid_param":"[{\"TB\":\"test_table.name\",\"OP\":\"L\",\"P\":\"John\"}]"
        }
        """;

        Assert.True(ixcOrm.ValidateQuery(expected));
    }
}
