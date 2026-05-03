using Microsoft.Extensions.Hosting;
using System.Net;
using DotNet.IXC.ORM.Test.Models;


namespace DotNet.IXC.ORM.Test;


public class RequestTest : IDisposable
{
    private readonly IHost host;


    public RequestTest()
    {
        host = Utils.MockedHostBuilder().Build();
    }


    public void Dispose()
    {
        host.Dispose();
        GC.SuppressFinalize(this);
    }


    [Fact]
    public async Task SuccessfulCustomerResponse()
    {
        string responseContent = @"{
            ""page"": ""1"",
            ""total"": ""5"",
            ""registros"": [
                {
                    ""id"": ""1"",
                    ""razao"": ""FELIPE S CARMO"",
                    ""cnpj_cpf"": ""123.456.789-10""
                }
            ]
        }".Replace("\n", "")
          .Replace("\r", "")
          .Replace("\t", "");

        using var client = Utils.MockedHttpClient(HttpStatusCode.OK, responseContent);
        using var orm = new IxcOrm("cliente", client);

        var content = await orm
            .Where("razao").Like("FELIPE S CARMO")
            .GetAsync<Cliente>();

        Assert.NotNull(content);
        Assert.Equal("success", content.Type);
        Assert.Equal(1, content.Page);
        Assert.Equal(5, content.Total);

        var customer = content.Records.FirstOrDefault();

        Assert.NotNull(customer);
        Assert.Equal(1, customer.Id);
        Assert.Equal("FELIPE S CARMO", customer.Razao);
        Assert.Equal("123.456.789-10", customer.CnpjCpf);
    }
}
