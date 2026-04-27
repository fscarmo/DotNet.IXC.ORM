using System.Net;
using System.Net.Http.Json;
using System.Numerics;
using System.Text;
using System.Text.Json;
using DotNet.IXC.ORM;
using DotNet.IXC.ORM.Config;
using DotNet.IXC.ORM.Exceptions;


namespace DotNet.IXC.ORM.Api;


public class RequestEmitter : IDisposable
{
    private readonly Dictionary<string, string> headers;
    private readonly HttpClient httpClient;


    private string url = string.Empty;


    protected string Table { get; private set; } = string.Empty;
    protected string Query { get; private set; } = string.Empty;


    public RequestEmitter(string table)
    {
        Table = table;

        headers = [];
        httpClient = IxcOrmOptions.Instance.HttpClient;

        SetupDefaultHeaders();
    }


    public async Task<IxcOrmResponse<T>?> GetAsync<T>() where T : class
    {
        SetupUrl();
        EnableIxcListingHeader();
        string content = await EmmitRequest(HttpMethod.Post);
        return new IxcOrmResponse<T>(content);
    }


    public async Task<IxcOrmResponse<T>?> PostAsync<T>() where T : class
    {
        SetupUrl();
        DisableIxcListingHeader();
        string content = await EmmitRequest(HttpMethod.Post);
        return new IxcOrmResponse<T>(content);
    }


    public async Task<IxcOrmResponse<T>?> PutAsync<T>(BigInteger id) where T : class
    {
        SetupUrl(id);
        DisableIxcListingHeader();
        string content = await EmmitRequest(HttpMethod.Put);
        return new IxcOrmResponse<T>(content);
    }


    public async Task<IxcOrmResponse<T>?> DeleteAsync<T>(BigInteger id) where T : class
    {
        SetupUrl(id);
        DisableIxcListingHeader();
        string content = await EmmitRequest(HttpMethod.Delete);
        return new IxcOrmResponse<T>(content);
    }


    public async Task<IxcResponse> EmmitResourceRequest(IDictionary<string, object> query)
    {
        try
        {
            SetupUrl();
            SetupQuery(JsonSerializer.Serialize(query));
            var response = await EmmitRequest(HttpMethod.Post);
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


    public void Dispose()
    {
        headers.Clear();
        httpClient.Dispose();
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


    protected virtual async Task<string> EmmitRequest(HttpMethod method, CancellationToken? cancellationToken = null)
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

        byte[] bytesToken = Encoding.UTF8.GetBytes(token);
        string base64Token = Convert.ToBase64String(bytesToken);

        headers["Authorization"] = $"Basic {base64Token}";
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
