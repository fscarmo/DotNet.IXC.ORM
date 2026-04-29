using Microsoft.Extensions.DependencyInjection;
using DotNet.IXC.ORM.Config;


namespace DotNet.IXC.ORM.Test;


public class EnvironmentTest
{
    [Fact]
    public void LoadEnvironmentFromAppSettings()
    {
        var host = Utils.MockedHostBuilderWithAppSettings().Build();
        var env = host.Services.GetService<IxcOrmEnvironment>();

        Assert.NotNull(env);

        var ixcAccessToken = env.IxcAccessToken;
        var ixcServerDomain = env.IxcServerDomain;

        Assert.NotNull(ixcAccessToken);
        Assert.NotNull(ixcServerDomain);
    }


    [Fact]
    public void LoadEnvironmentFromSystem()
    {
        var host = Utils.MockedHostBuilderWithEnvironment().Build();
        var env = host.Services.GetService<IxcOrmEnvironment>();

        Assert.NotNull(env);

        var ixcAccessToken = env.IxcAccessToken;
        var ixcServerDomain = env.IxcServerDomain;

        Assert.NotNull(ixcAccessToken);
        Assert.NotNull(ixcServerDomain);
    }
}
