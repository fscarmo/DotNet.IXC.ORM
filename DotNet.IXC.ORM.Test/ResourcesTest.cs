using Microsoft.Extensions.Hosting;
using System.Net;


namespace DotNet.IXC.ORM.Test;


public class ResourcesTest : IDisposable
{
    private readonly IHost host;


    public ResourcesTest()
    {
        host = Utils.MockedHostBuilder().Build();
    }


    public void Dispose()
    {
        host.Dispose();
        GC.SuppressFinalize(this);
    }


    [Fact]
    public async Task DesbloqueioDeConfianca()
    {
        string responseContent = @"{
            ""type"": ""success"",
            ""message"": ""O contrato foi desbloqueado com sucesso por 3 dias."",
            ""get_id"": ""12345""
        }".Replace("\n", "")
          .Replace("\r", "")
          .Replace("\t", "");

        using var client = Utils.MockedHttpClient(HttpStatusCode.OK, responseContent);

        var response = await IxcResource.DesbloqueioDeConfianca(12345, client);

        Assert.NotNull(response);
        Assert.Matches("success|sucesso", response.Type);
        Assert.Contains("O contrato foi desbloqueado com sucesso por 3 dias.", response.Message);
    }


    [Fact]
    public async Task LiberacaoTemporariaTest()
    {
        string responseContent = @"{
            ""codigo"": ""200"",
            ""dias"": ""3"",
            ""mensagem"": ""O contrato foi desbloqueado com sucesso por 3 dias. Caso não regularize a situação financeira até..."",
            ""tipo"": ""sucesso""
        }".Replace("\n", "")
          .Replace("\r", "")
          .Replace("\t", "");

        using var client = Utils.MockedHttpClient(HttpStatusCode.OK, responseContent);

        var response = await IxcResource.LiberacaoTemporaria(12345, client);

        Assert.NotNull(response);
        Assert.Matches("success|sucesso", response.Type);
        Assert.Contains("O contrato foi desbloqueado com sucesso por 3 dias.", response.Message);
    }


    [Fact]
    public async Task LimparMacTest()
    {
        string responseContent = @"{
            ""type"": ""success"",
            ""message"": ""MAC removido com sucesso para o(s) usuário(s) com ID: 12345<br />Ação executada com sucesso"",
            ""get_id"": ""12345""
        }";

        using var client = Utils.MockedHttpClient(HttpStatusCode.OK, responseContent);

        var response = await IxcResource.LimparMac(12345, client);

        Assert.NotNull(response);
        Assert.Matches("success|sucesso", response.Type);
        Assert.Contains("MAC removido com sucesso para o(s) usuário(s) com ID: 12345", response.Message);
    }
}
