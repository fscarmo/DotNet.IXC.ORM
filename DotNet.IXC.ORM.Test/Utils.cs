using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Moq.Protected;
using System.Net;
using DotNet.IXC.ORM.Config;


namespace DotNet.IXC.ORM.Test;


public static class Utils
{
    public static IHostBuilder MockedHostBuilder()
    {
        return new HostBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json");
            })
            .ConfigureServices((context, services) =>
            {
                var section = context.Configuration.GetSection("IxcOrm")
                    ?? throw new Exception("A seção IxcOrm está faltando no arquivo appsettings.json.");

                services.AddIxcOrmEnvironment(env =>
                {
                    string? ixcAccessToken = section["IxcAccessToken"];
                    string? ixcServerDomain = section["IxcServerDomain"];

                    if (!string.IsNullOrWhiteSpace(ixcAccessToken) &&
                        !string.IsNullOrWhiteSpace(ixcServerDomain))
                    {
                        env.SetupAccessToken(ixcAccessToken);
                        env.SetupServerDomain(ixcServerDomain);
                    }
                });
            });
    }


    public static IHostBuilder MockedHttpMessageHandler(
        IHostBuilder hostBuilder,
        HttpStatusCode? statusCode = null,
        string? response = null
    )
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = statusCode ?? HttpStatusCode.OK,
                Content = new StringContent(response ?? """{"type":"success","page":0,"total":0,"registros":[]}"""),
            });

        return hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddHttpClient(options =>
            {
                var httpClient = new HttpClient(handlerMock.Object);
                options.SetupHttpClient(httpClient);
            });
        });
    }
}
