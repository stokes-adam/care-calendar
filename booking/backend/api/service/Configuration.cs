using System.Text.Json;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using model.interfaces;

namespace service;

public class Configuration : IConfiguration
{
    public string ConnectionString { get; }
    public RegionEndpoint Region { get; }
    public string EncryptionKeyId { get; }
    
    public Configuration()
    {
        ConnectionString = GetConnectionString();
        Region = GetRegion();
        EncryptionKeyId = Environment.GetEnvironmentVariable("EncryptionKeyId") ?? throw new Exception("EncryptionKeyId not set");
    }
    
    private RegionEndpoint GetRegion()
    {
        var regionString = Environment.GetEnvironmentVariable("Region") ?? throw new Exception("Region not set");
        var region = RegionEndpoint.GetBySystemName(regionString);
        
        return region;
    }

    private string GetConnectionString()
    {
        var secretsManagerClient = new AmazonSecretsManagerClient();
        var secretRequest = new GetSecretValueRequest { SecretId = "ConnectionString" };
        var secretResponse = secretsManagerClient.GetSecretValueAsync(secretRequest).Result;
        var secretData = JsonSerializer.Deserialize<Dictionary<string, string>>(secretResponse.SecretString);
        
        return secretData?["ConnectionString"] ?? throw new Exception("ConnectionString not set");
    }
}