using DotNet.IXC.ORM.Enums;


namespace DotNet.IXC.ORM.Test;


public class QueryBuilderTest
{
    [Fact]
    public void QueryWithLikeOperator()
    {
        Utils.BuildHost();

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
        """.Replace("\r", "").Replace("\n", "").Replace(" ", "");

        string actual = ixcOrm.GetQueryAsJsonString()
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace(" ", "");

        Assert.Equal(expected, actual);
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
        """.Replace("\r", "").Replace("\n", "").Replace(" ", "");

        string actual = ixcOrm.GetQueryAsJsonString()
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace(" ", "");

        Assert.Equal(expected, actual);
    }
}
