namespace DotNet.IXC.ORM.Test;


public class RecursosTest
{
    public RecursosTest()
    {
        Utils.MockedHostBuilder().Build();
    }


    [Fact]
    public async Task DesbloqueioDeConfianca()
    {
        var response = await IxcResource.DesbloqueioDeConfianca(9326);

        Assert.NotNull(response);
        Assert.Matches("success|sucesso", response.Type);
        Assert.Contains("O contrato foi desbloqueado com sucesso por 3 dias.", response.Message);
    }


    [Fact]
    public async Task FalhaAoTentarAtivarContratoJaAtivoTest()
    {
        var response = await IxcResource.AtivarContrato(8466);

        Assert.NotNull(response);
        Assert.Equal("error", response.Type);
        Assert.Contains("Não foi possível ativar o contrato ID: ", response.Message);
    }


    [Fact]
    public async Task LiberacaoTemporariaTest()
    {
        var response = await IxcResource.LiberacaoTemporaria(9428);

        Assert.NotNull(response);
        Assert.Matches("success|sucesso", response.Type);
        Assert.Contains("O contrato foi desbloqueado com sucesso por 3 dias.", response.Message);
    }


    [Fact]
    public async Task LimparMacTest()
    {
        var response = await IxcResource.LimparMac(9159);

        Assert.NotNull(response);
        Assert.Equal("success", response.Type);
        Assert.Contains("MAC removido com sucesso", response.Message);
    }
}
