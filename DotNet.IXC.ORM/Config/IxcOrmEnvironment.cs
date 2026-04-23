using DotNet.IXC.ORM.Exceptions;


namespace DotNet.IXC.ORM.Config;


public sealed class IxcOrmEnvironment
{
    private static readonly IxcOrmEnvironment _instance =
        new Lazy<IxcOrmEnvironment>(() => new IxcOrmEnvironment()).Value;


    public static IxcOrmEnvironment Instance => _instance;


    public string? IxcAccessToken { get; private set; } = null;
    public string? IxcServerDomain { get; private set; } = null;


    private IxcOrmEnvironment()
    {
    }


    public IxcOrmEnvironment SetupAccessToken(string ixcAccessToken)
    {
        if (string.IsNullOrWhiteSpace(ixcAccessToken))
        {
            throw new IxcOrmArgumentException(nameof(ixcAccessToken));
        }

        if (string.IsNullOrWhiteSpace(IxcAccessToken))
        {
            IxcAccessToken = ixcAccessToken;
        }

        return this;
    }


    public IxcOrmEnvironment SetupServerDomain(string ixcServerDomain)
    {
        if (string.IsNullOrWhiteSpace(ixcServerDomain))
        {
            throw new IxcOrmArgumentException(nameof(ixcServerDomain));
        }

        if (string.IsNullOrWhiteSpace(IxcServerDomain))
        {
            IxcServerDomain = ixcServerDomain;
        }

        return this;
    }
}
