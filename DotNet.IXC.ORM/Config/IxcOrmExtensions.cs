using Microsoft.Extensions.DependencyInjection;


namespace DotNet.IXC.ORM.Config;


public static class IxcOrmExtensions
{
    public static IServiceCollection AddIxcOrmEnvironment(this IServiceCollection services, Action<IxcOrmEnvironment> configure)
    {
        var env = IxcOrmEnvironment.Instance;
        configure(env);
        services.AddSingleton(env);
        return services;
    }


    public static IServiceCollection AddHttpClient(this IServiceCollection services, Action<IxcOrmOptions> configure)
    {
        var options = IxcOrmOptions.Instance;
        configure(options);
        services.AddSingleton(options);
        return services;
    }
}
