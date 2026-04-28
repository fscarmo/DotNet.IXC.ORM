using DotNet.IXC.ORM.Api;


namespace DotNet.IXC.ORM;


public abstract class IxcResource
{
    public static readonly string SRC_ATIVAR_CLIENTE = "cliente_contrato_ativar_cliente";
    public static readonly string SRC_DESBLOQUEIO_CONFIANCA = "desbloqueio_confianca";
    public static readonly string SRC_LIBERACAO_TEMPORARIA = "cliente_contrato_btn_lib_temp_24722";
    public static readonly string SRC_LIMPAR_MAC = "radusuarios_25452";


    public static async Task<IxcResponse> AtivarContrato(int idContrato)
    {
        if (idContrato < 1)
            throw new ArgumentException("O id do contrato deve ser maior que zero.", nameof(idContrato));

        var query = new Dictionary<string, object>
        {
            { "qtype", "cliente_contrato_ativar_cliente.id" },
            { "id_contrato",  idContrato }
        };

        using var emitter = new RequestEmitter(SRC_ATIVAR_CLIENTE);
        return await emitter.EmmitResourceRequestAsync(query);
    }


    public static async Task<IxcResponse> DesbloqueioDeConfianca(int idContrato)
    {
        if (idContrato < 1)
            throw new ArgumentException("O id do contrato deve ser maior que zero.", nameof(idContrato));

        var query = new Dictionary<string, object>
        {
            { "id", idContrato }
        };

        using var emitter = new RequestEmitter(SRC_DESBLOQUEIO_CONFIANCA);
        return await emitter.EmmitResourceRequestAsync(query);
    }


    public static async Task<IxcResponse> LiberacaoTemporaria(int idContrato)
    {
        if (idContrato < 1)
            throw new ArgumentException("O id do contrato deve ser maior que zero.", nameof(idContrato));

        var query = new Dictionary<string, object>
        {
            { "id", idContrato }
        };

        using var emitter = new RequestEmitter(SRC_LIBERACAO_TEMPORARIA);
        return await emitter.EmmitResourceRequestAsync(query);
    }


    public static async Task<IxcResponse> LimparMac(int idLogin)
    {
        if (idLogin < 1)
            throw new ArgumentException("O id do PPPoE deve ser maior que zero.", nameof(idLogin));

        var query = new Dictionary<string, object>
        {
            { "get_id",  idLogin }
        };

        using var emitter = new RequestEmitter(SRC_LIMPAR_MAC);
        return await emitter.EmmitResourceRequestAsync(query);
    }
}
