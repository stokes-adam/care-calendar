using Amazon;
using Amazon.KeyManagementService;
using model.interfaces;
using service;
using service.options;
using service.postgres;

namespace api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAwsServices(this IServiceCollection services)
    {
        var regionString = Environment.GetEnvironmentVariable("Region")
                           ?? throw new Exception("Region not set");
        
        var region = RegionEndpoint.GetBySystemName(regionString);
        
        return services
            .AddSingleton<IAmazonKeyManagementService>(_ => new AmazonKeyManagementServiceClient(region))
            .AddSingleton<IEncryption, AwsEncryption>();
    }
    
    public static IServiceCollection AddPostgresServices(this IServiceCollection services)
    {
        return services
            .AddScoped<ICommandHandler, PostgresCommandHandler>();
    }

    public static IServiceCollection AddLocalEnvironment(this IServiceCollection services)
    {
        return services
            .AddSingleton<IEncryption, NoEncryption>();
    }
}