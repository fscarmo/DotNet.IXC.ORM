using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Moq.Protected;
using System.Net;
using DotNet.IXC.ORM.Config;


namespace DotNet.IXC.ORM.Test;


public class Utils
{
    public static IHostBuilder MockedHostBuilder()
    {
        return new HostBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                var section = context.Configuration.GetSection("IxcOrm")
                    ?? throw new Exception("A seção \"IxcOrm__\" não está presente nas variáveis de ambiente.");

                services.AddIxcOrmEnvironment(env =>
                {
                    string? ixcAccessToken = section["AccessToken"];
                    string? ixcServerDomain = section["ServerDomain"];

                    if (!string.IsNullOrWhiteSpace(ixcAccessToken) &&
                        !string.IsNullOrWhiteSpace(ixcServerDomain))
                    {
                        env.SetupAccessToken(ixcAccessToken);
                        env.SetupServerDomain(ixcServerDomain);
                    }
                });
            });
    }


    public static HttpClient MockedHttpClient(HttpStatusCode? statusCode = null, string? response = null)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handlerMock.Protected()
            .Setup("Dispose", ItExpr.IsAny<bool>());

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
                Content = new StringContent(response ?? """{"type":"success","message":"Teste!"}"""),
            });

        return new HttpClient(handlerMock.Object);
    }
}
