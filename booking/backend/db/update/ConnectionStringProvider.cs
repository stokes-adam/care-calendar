using Amazon.KeyManagementService;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace update;

public class ConnectionStringProvider
{
    private const string DefaultLocalConnectionString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres;";

    public static string GetConnectionString(IEnumerable<string> args)
    {
        var local = args.FirstOrDefault() == "local";

        return local ? Local() : Remote();
    }
    private static string Local()
    {
        return DefaultLocalConnectionString;
    }
    
    private static string Remote()
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