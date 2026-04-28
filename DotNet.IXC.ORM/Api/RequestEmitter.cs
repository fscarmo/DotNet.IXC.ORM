using System.Net;
using System.Numerics;
using System.Text;
using System.Text.Json;
using DotNet.IXC.ORM.Config;
using DotNet.IXC.ORM.Exceptions;


namespace DotNet.IXC.ORM.Api;


/// <summary>
/// A classe <c>RequestEmitter</c> constrói o a estrutura das requisições e as executa, além de fornecer acesso
/// padronizado aos métodos de requisição da API do IXC Provedor.
/// </summary>
/// <remarks>
/// Autor: Felipe S. Carmo <br/>
/// Versão: 1.0.0 <br/>
/// Desde: 2026-04-28
/// </remarks>
public class RequestEmitter : IDisposable
{
    private static readonly string AUTH_HEADER_KEY = "Authorization";
    private static readonly string IXCSOFT_HEADER_KEY = "ixcsoft";
    private static readonly string IXCSOFT_HEADER_VALUE = "listar";


    private readonly Dictionary<string, string> headers;
    private readonly HttpClient httpClient;
    private string url = string.Empty;
    private bool disposed;


    protected string Table { get; private set; } = string.Empty;
    protected string Query { get; private set; } = string.Empty;


    /// <include file='Docs/Api/RequestEmitter.xml' path='docs/member[@name="Constructor"]/*'/>
    public RequestEmitter(string table)
    {
        Table = table;

        headers = [];
        httpClient = IxcOrmOptions.Instance.HttpClient;

        SetupDefaultHeaders();
    }


    /// <include file='Docs/Api/RequestEmitter.xml' path='docs/member[@name="GetAsync"]/*'/>
    public async Task<IxcOrmResponse<T>> GetAsync<T>() where T : class
    {
        SetupUrl();

        headers[IXCSOFT_HEADER_KEY] = IXCSOFT_HEADER_VALUE;

        string content = await EmmitRequestAsync(HttpMethod.Post);
        return new IxcOrmResponse<T>(content);
    }


    /// <include file='Docs/Api/RequestEmitter.xml' path='docs/member[@name="PostAsync"]/*'/>
    public async Task<IxcResponse> PostAsync(object record)
    {
        SetupUrl();

        headers[IXCSOFT_HEADER_KEY] = string.Empty;

        string content = await EmmitRequestAsync(HttpMethod.Post);
        return new IxcResponse(content);
    }


    /// <include file='Docs/Api/RequestEmitter.xml' path='docs/member[@name="PutAsync"]/*'/>
    public async Task<IxcResponse> PutAsync(BigInteger id, object record)
    {
        SetupUrl(id);

        headers[IXCSOFT_HEADER_KEY] = string.Empty;

        string content = await EmmitRequestAsync(HttpMethod.Put);
        return new IxcResponse(content);
    }


    /// <include file='Docs/Api/RequestEmitter.xml' path='docs/member[@name="DeleteAsync"]/*'/>
    public async Task<IxcResponse> DeleteAsync(BigInteger id)
    {
        SetupUrl(id);

        headers[IXCSOFT_HEADER_KEY] = string.Empty;

        string content = await EmmitRequestAsync(HttpMethod.Delete);
        return new IxcResponse(content);
    }


    /// <include file='Docs/Api/RequestEmitter.xml' path='docs/member[@name="EmmitResourceRequestAsync"]/*'/>
    public async Task<IxcResponse> EmmitResourceRequestAsync(IDictionary<string, object> query)
    {
        try
        {
            SetupUrl();
            SetupQuery(JsonSerializer.Serialize(query));
            var response = await EmmitRequestAsync(HttpMethod.Post);
            return new IxcResponse(response);
        }
        catch (JsonException e)
        {
            string content = IxcResponse.CreateResponseContentError(e.Message);
            return new IxcResponse(content);
        }
        catch (IxcOrmRequestException e)
        {
            string content = IxcResponse.CreateResponseContentError(e.Message);
            return new IxcResponse(content);
        }
    }


    /// <include file='Docs/Api/RequestEmitter.xml' path='docs/member[@name="Dispose"]/*'/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    /// <include file='Docs/Api/RequestEmitter.xml' path='docs/member[@name="Dispose.Disposing"]/*'/>
    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;

        if (disposing)
        {
            headers.Clear();
            httpClient.Dispose();
        }

        disposed = true;
    }


    /// <include file='Docs/Api/RequestEmitter.xml' path='docs/member[@name="SetupQuery"]/*'/>
    protected void SetupQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Query = string.Empty;
            return;
        }

        try
        {
            using var json = JsonDocument.Parse(query);
            Query = query;
        }
        catch (JsonException e)
        {
            throw new ArgumentException(
                "A query de busca está em um formato JSON inválido.", nameof(query), e);
        }
    }


    /// <include file='Docs/Api/RequestEmitter.xml' path='docs/member[@name="EmmitRequestAsync"]/*'/>
    protected virtual async Task<string> EmmitRequestAsync(HttpMethod method, CancellationToken? cancellationToken = null)
    {
        HttpResponseMessage? response = null;

        try
        {
            using var request = new HttpRequestMessage(method, url);
            request.Content = new StringContent(Query, Encoding.UTF8, "application/json");

            headers.ToList().ForEach(header =>
                request.Headers.TryAddWithoutValidation(header.Key, header.Value)
            );

            response = await httpClient.SendAsync(request, cancellationToken ?? CancellationToken.None);
            response.EnsureSuccessStatusCode();

            return await response
                .Content
                .ReadAsStringAsync(cancellationToken ?? CancellationToken.None);
        }
        catch (TaskCanceledException e)
        {
            throw new IxcOrmRequestException(
                "Falha na conexão com o servidor. O tempo limite da requisição foi excedido.", e);
        }
        catch (InvalidOperationException e)
        {
            throw new IxcOrmRequestException(
                "Falha na operação. A requisição HTTP não pôde ser enviada para o IXC.", e);
        }
        catch (HttpRequestException e)
        {
            int statusCode = (int)(response?.StatusCode ?? HttpStatusCode.ServiceUnavailable);
            throw new IxcOrmRequestException(
                $"A requisição HTTP falhou. O IXC retornou um código de erro: {statusCode}", e);
        }
        finally
        {
            response?.Dispose();
        }
    }


    private void SetupDefaultHeaders()
    {
        string token = IxcOrmEnvironment.Instance.IxcAccessToken
            ?? throw new IxcOrmEnvironmentException("IxcAccessToken");

        byte[] bytesToken = Encoding.UTF8.GetBytes(token);
        string base64Token = Convert.ToBase64String(bytesToken);

        headers[AUTH_HEADER_KEY] = $"Basic {base64Token}";
        headers[IXCSOFT_HEADER_KEY] = string.Empty;
    }


    private void SetupUrl()
    {
        string domain = IxcOrmEnvironment.Instance.IxcServerDomain
            ?? throw new IxcOrmEnvironmentException("IxcServerDomain");

        url = $"https://{domain}/webservice/v1/{Table}/";
    }


    private void SetupUrl(BigInteger id)
    {
        string domain = IxcOrmEnvironment.Instance.IxcServerDomain
            ?? throw new IxcOrmEnvironmentException("IxcServerDomain");

        url = $"https://{domain}/webservice/v1/{Table}/{id}";
    }
}
