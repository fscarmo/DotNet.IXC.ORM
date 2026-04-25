namespace DotNet.IXC.ORM.Config;


public sealed class IxcOrmOptions
{
    private static readonly IxcOrmOptions _instance =
        new Lazy<IxcOrmOptions>(() => new IxcOrmOptions()).Value;


    public static IxcOrmOptions Instance => _instance;


    public HttpClient HttpClient { get; private set; } = new HttpClient();


    public void SetupHttpClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }
}
