using model.interfaces;
using service;
using service.aws;
using service.postgres;

namespace api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnvironmentSpecificServices(this IServiceCollection services, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            services.AddSingleton<IEncryption, NoEncryption>();
            services.AddSingleton<IConfigManager, LocalConfigManager>();
        }
        else
        {
            services.AddSingleton<IEncryption, AwsEncryption>();
            services.AddSingleton<IConfigManager, AwsConfigManager>();
        }
        
        return services;
    }
}