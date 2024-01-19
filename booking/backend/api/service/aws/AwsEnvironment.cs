using Amazon;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using model.interfaces;

namespace service;

public class AwsEnvironment : IEnvironment
{
    public string ConnectionString { get; } = GetConnectionString();
    public RegionEndpoint Region { get; } = GetRegion();
    
    private static RegionEndpoint GetRegion()
    {
        var regionString = Environment.GetEnvironmentVariable("Region")
                           ?? throw new Exception("Region not set");
        
        var region = RegionEndpoint.GetBySystemName(regionString);
        
        return region;
    }

    private static string GetConnectionString()
    {
        var service = new AmazonSimpleSystemsManagementClient();
        
        var request = new GetParameterRequest
        {
            Name = "/care-calendar/db-connection-string"
        };
        
        var response = service.GetParameterAsync(request).Result;
        
        var connectionString = response.Parameter.Value;
        
        return connectionString;
    }
}