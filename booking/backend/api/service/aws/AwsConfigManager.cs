using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using model.interfaces;

namespace service.aws;

public class AwsConfigManager : IConfigManager
{
    public string ConnectionString { get; } = GetConnectionStringAsync()
                                              ?? throw new Exception("ConnectionString not set");

    private static string GetConnectionStringAsync()
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