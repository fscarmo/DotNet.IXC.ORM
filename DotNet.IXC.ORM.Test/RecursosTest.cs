namespace DotNet.IXC.ORM.Test;


public class RecursosTest
{
    public RecursosTest()
    {
        Utils.MockedHostBuilder().Build();
    }


    [Fact]
    public async Task FalhaAoTentarAtivarContratoJaAtivoTest()
    {
        var response = await IxcRecurso.AtivarContrato(8466);

        Assert.NotNull(response);
        Assert.Equal("error", response.Type);
        Assert.Contains("Não foi possível ativar o contrato ID: ", response.Message);
    }
}
