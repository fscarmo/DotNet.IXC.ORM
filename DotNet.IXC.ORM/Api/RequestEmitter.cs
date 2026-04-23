using DotNet.IXC.ORM.Config;
using DotNet.IXC.ORM.Exceptions;
using System.Reflection.PortableExecutable;


namespace DotNet.IXC.ORM.Api;


public class RequestEmitter : IDisposable
{
    private readonly Dictionary<string, string> headers = [];
    private readonly HttpClient httpClient;
    private readonly string table;
    private string url;


    protected RequestEmitter(string table)
    {
        this.httpClient = new HttpClient();
        this.table = table;
        this.url = string.Empty;

        SetupDefaultHeaders();
    }


    public void Dispose()
    {
        headers.Clear();
        httpClient.Dispose();
        url = string.Empty;
    }


    protected async Task<string> EmmitRequest(HttpMethod method)
    {
        ObjectDisposedException.ThrowIf(httpClient == null, nameof(httpClient));

        var response = await httpClient.SendAsync(new HttpRequestMessage(method, url)
        {
            Headers =
                {
                    { "Authorization", headers["Authorization"] },
                    { "Content-Type", headers["Content-Type"] },
                    { "ixcsoft", headers["ixcsoft"] }
                }
        });

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }


    private void SetupDefaultHeaders()
    {
        string token = IxcOrmEnvironment.Instance.IxcAccessToken
            ?? throw new IxcOrmEnvironmentException("IxcAccessToken");

        headers["Authorization"] = $"Basic {token}";
        headers["Content-Type"] = "application/json";
        headers["ixcsoft"] = string.Empty;
    }


    private void SetupUrl()
    {
        string domain = IxcOrmEnvironment.Instance.IxcServerDomain
            ?? throw new IxcOrmEnvironmentException("IxcServerDomain");
        url = $"https://{domain}/webservice/v1/{table}/";
    }


    private void SetupUrl(int id)
    {
        string domain = IxcOrmEnvironment.Instance.IxcServerDomain
            ?? throw new IxcOrmEnvironmentException("IxcServerDomain");
        url = $"https://{domain}/webservice/v1/{table}/{id}";
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
