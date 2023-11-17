using System.Security.Cryptography;
using System.Text;
using Amazon;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Microsoft.Extensions.Logging;
using model.interfaces;

namespace service;

public class AwsEncryption : IEncryption
{
    private readonly ILogger<AwsEncryption> _logger;
    private readonly AmazonKeyManagementServiceClient _kmsClient;
    private readonly string _kmsKeyId;

    public AwsEncryption(ILogger<AwsEncryption> logger)
    {
        _logger = logger;
        _kmsClient = new AmazonKeyManagementServiceClient(GetRegion());
        _kmsKeyId = Environment.GetEnvironmentVariable("EncryptionKeyId")
                          ?? throw new Exception("EncryptionKeyId not set");
    }

    public async Task<string> Encrypt(string value)
    {
        try
        {
            var encryptionRequest = new EncryptRequest
            {
                KeyId = _kmsKeyId,
                Plaintext = new MemoryStream(Encoding.UTF8.GetBytes(value))
            };

            var encryptionResponse = await _kmsClient.EncryptAsync(encryptionRequest);
            
            var result = Convert.ToBase64String(encryptionResponse.CiphertextBlob.ToArray());
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to encrypt value");
            throw;
        }
    }

    public async Task<string> Decrypt(string value)
    {
        try
        {
            var decryptionRequest = new DecryptRequest
            {
                CiphertextBlob = new MemoryStream(Convert.FromBase64String(value))
            };
            
            var decryptionResponse = await _kmsClient.DecryptAsync(decryptionRequest);
            
            var result = Encoding.UTF8.GetString(decryptionResponse.Plaintext.ToArray());
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to decrypt value");
            throw;
        }
    }

    private static RegionEndpoint GetRegion()
    {
        var regionString = Environment.GetEnvironmentVariable("Region")
                           ?? throw new Exception("Region not set");

        var region = RegionEndpoint.GetBySystemName(regionString);

        return region;
    }
}