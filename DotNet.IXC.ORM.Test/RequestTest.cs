using DotNet.IXC.ORM.Test.Models;


namespace DotNet.IXC.ORM.Test;


public class RequestTest
{
    public RequestTest()
    {
        Utils.BuildHost();
    }


    [Fact]
    public async Task ListClients()
    {
        var orm = new IxcOrm("cliente");
        var content = await orm.Where("razao").Like("FELIPE DE SOUSA").GetAsync<Client>();

        Assert.NotNull(content);
    }
}
