using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DotNet.IXC.ORM.Config;


namespace DotNet.IXC.ORM.Test
{
    public class EnvironmentTest
    {
        [Fact]
        public void LoadEnvironmentService()
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json");
                })
                .ConfigureServices((context, services) =>
                {
                    var section = context.Configuration.GetSection("IxcOrm");
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
                })
                .Build();

            var env = host.Services.GetService<IxcOrmEnvironment>();

            Assert.NotNull(env);

            var ixcAccessToken = env.IxcAccessToken;
            var ixcServerDomain = env.IxcServerDomain;

            Assert.NotNull(ixcAccessToken);
            Assert.NotNull(ixcServerDomain);
        }
    }
}
