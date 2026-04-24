using System.Net;
using System.Numerics;
using System.Text;
using System.Text.Json;
using DotNet.IXC.ORM.Config;
using DotNet.IXC.ORM.Exceptions;


namespace DotNet.IXC.ORM.Api;


public class RequestEmitter : IDisposable
{
    private readonly Dictionary<string, string> headers = [];
    private readonly HttpClient httpClient = new();
    private string url = string.Empty;


    protected string Table { get; private set; } = string.Empty;
    protected string Query { get; private set; } = string.Empty;


    protected RequestEmitter(string table)
    {
        Table = table;
        SetupDefaultHeaders();
    }


    public async Task<IxcResponse> GetAsync()
    {
        SetupUrl();
        EnableIxcListingHeader();
        var response = await EmmitRequest(HttpMethod.Post);
        return new IxcResponse(response);
    }


    public async Task<IxcResponse> PostAsync()
    {
        SetupUrl();
        DisableIxcListingHeader();
        var response = await EmmitRequest(HttpMethod.Post);
        return new IxcResponse(response);
    }


    public async Task<IxcResponse> PutAsync(BigInteger id)
    {
        SetupUrl(id);
        DisableIxcListingHeader();
        var response = await EmmitRequest(HttpMethod.Put);
        return new IxcResponse(response);
    }


    public async Task<IxcResponse> DeleteAsync(BigInteger id)
    {
        SetupUrl(id);
        DisableIxcListingHeader();
        var response = await EmmitRequest(HttpMethod.Delete);
        return new IxcResponse(response);
    }


    public void Dispose()
    {
        headers.Clear();
        httpClient.Dispose();
        url = string.Empty;
    }


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
                "A query de busca não está em um formato JSON válido.", nameof(query), e);
        }
    }


    protected async Task<HttpContent> EmmitRequest(HttpMethod method)
    {
        HttpResponseMessage? response = null;

        try
        {
            using var request = new HttpRequestMessage(method, url);
            request.Content = new StringContent(Query, Encoding.UTF8, "application/json");

            headers.ToList().ForEach(header =>
                request.Headers.TryAddWithoutValidation(header.Key, header.Value)
            );

            response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response.Content;
        }
        catch (TaskCanceledException e)
        {
            throw new IxcOrmRequestException(
                "Falha na conexão com o servidor. O tempo limite da requisição HTTP foi excedido.", e);
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

        headers["Authorization"] = $"Basic {token}";
        headers["ixcsoft"] = string.Empty;
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


    private void EnableIxcListingHeader()
    {
        headers["ixcsoft"] = "listar";
    }


    private void DisableIxcListingHeader()
    {
        headers["ixcsoft"] = string.Empty;
    }
}
