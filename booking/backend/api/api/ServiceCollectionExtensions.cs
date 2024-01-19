using model.interfaces;
using service;
using service.postgres;

namespace api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnvironmentSpecificServices(this IServiceCollection services, bool isDevelop)
    {
        if (isDevelop)
        {
            services.AddSingleton<IEncryption, LocalEncryption>();
            services.AddSingleton<IEnvironment, LocalEnvironment>();
        }
        else
        {
            services.AddSingleton<IEncryption, AwsEncryption>();
            services.AddSingleton<IEnvironment, AwsEnvironment>();
        }
        
        return services;
    }
}