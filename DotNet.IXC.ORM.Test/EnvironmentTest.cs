using Microsoft.Extensions.DependencyInjection;
using DotNet.IXC.ORM.Config;


namespace DotNet.IXC.ORM.Test;


public class EnvironmentTest
{
    [Fact]
    public void LoadEnvironmentFromSystem()
    {
        using var host = Utils.MockedHostBuilder().Build();

        var env = host.Services.GetService<IxcOrmEnvironment>();

        Assert.NotNull(env);

        var ixcAccessToken = env.IxcAccessToken;
        var ixcServerDomain = env.IxcServerDomain;

        Assert.NotNull(ixcAccessToken);
        Assert.NotNull(ixcServerDomain);
    }
}
