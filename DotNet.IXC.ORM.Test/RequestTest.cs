using System.Net;
using DotNet.IXC.ORM.Test.Models;


namespace DotNet.IXC.ORM.Test;


public class RequestTest
{
    [Fact]
    public async Task SuccessfulCustomerResponse()
    {
        string response = @"{
            ""type"": ""success"",
            ""page"": 1,
            ""total"": 20,
            ""registros"": [
                {
                    ""id"": 1,
                    ""razao"": ""FELIPE S CARMO"",
                    ""cnpj_cpf"": ""123.456.789-10""
                }
            ]
        }".Replace("\r", "")
          .Replace("\n", "");

        Utils.MockedHttpMessageHandler(
            Utils.MockedHostBuilder(),
            HttpStatusCode.OK,
            response
        ).Build();

        var orm = new IxcOrm("cliente");

        var content = await orm
            .Where("razao").Like("FELIPE DE SOUSA")
            .GetAsync<Customer>();

        Assert.NotNull(content);
        Assert.Equal("success", content.Type);
        Assert.Equal(1, content.Page);
        Assert.Equal(20, content.Total);

        var client = content.Records.FirstOrDefault();

        Assert.NotNull(client);
        Assert.Equal(1, client.Id);
        Assert.Equal("FELIPE S CARMO", client.Razao);
        Assert.Equal("123.456.789-10", client.CnpjCpf);
    }
}
