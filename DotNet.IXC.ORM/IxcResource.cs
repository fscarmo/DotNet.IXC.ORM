using DotNet.IXC.ORM.Api;


namespace DotNet.IXC.ORM;


public abstract class IxcResource
{
    public static readonly string SRC_ATIVAR_CONTRATO = "cliente_contrato_ativar_cliente";
    public static readonly string SRC_DESBLOQUEIO_CONFIANCA = "desbloqueio_confianca";
    public static readonly string SRC_LIBERACAO_TEMPORARIA = "cliente_contrato_btn_lib_temp_24722";
    public static readonly string SRC_LIMPAR_MAC = "radusuarios_25452";


    public static async Task<IxcResponse> AtivarContrato(int contrato, HttpClient? client = null)
    {
        if (contrato < 1)
            throw new ArgumentException("O id do contrato deve ser maior que zero.", nameof(contrato));

        var query = new Dictionary<string, object>
        {
            { "qtype", $"{SRC_ATIVAR_CONTRATO}.id" },
            { "id_contrato",  contrato }
        };

        using var emitter = new RequestEmitter(SRC_ATIVAR_CONTRATO, client ?? new HttpClient());
        return await emitter.EmmitResourceRequestAsync(query);
    }


    public static async Task<IxcResponse> DesbloqueioDeConfianca(int contrato, HttpClient? client = null)
    {
        if (contrato < 1)
            throw new ArgumentException("O id do contrato deve ser maior que zero.", nameof(contrato));

        var query = new Dictionary<string, object>
        {
            { "id", contrato }
        };

        using var emitter = new RequestEmitter(SRC_DESBLOQUEIO_CONFIANCA, client ?? new HttpClient());
        return await emitter.EmmitResourceRequestAsync(query);
    }


    public static async Task<IxcResponse> LiberacaoTemporaria(int contrato, HttpClient? client = null)
    {
        if (contrato < 1)
            throw new ArgumentException("O id do contrato deve ser maior que zero.", nameof(contrato));

        var query = new Dictionary<string, object>
        {
            { "id", contrato }
        };

        using var emitter = new RequestEmitter(SRC_LIBERACAO_TEMPORARIA, client ?? new HttpClient());
        return await emitter.EmmitResourceRequestAsync(query);
    }


    public static async Task<IxcResponse> LimparMac(int login, HttpClient? client = null)
    {
        if (login < 1)
            throw new ArgumentException("O id do login (PPPoE) deve ser maior que zero.", nameof(login));

        var query = new Dictionary<string, object>
        {
            { "get_id",  login }
        };

        using var emitter = new RequestEmitter(SRC_LIMPAR_MAC, client ?? new HttpClient());
        return await emitter.EmmitResourceRequestAsync(query);
    }
}
