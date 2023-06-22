using System.Security.Cryptography;
using System.Text;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Microsoft.Extensions.Logging;
using model.interfaces;

namespace service;

public class Encryption
{
    private readonly ILogger<Encryption> _logger;
    private readonly AmazonKeyManagementServiceClient _kmsClient;
    private readonly string _kmsKeyId;

    public Encryption(ILogger<Encryption> logger, IConfiguration configuration)
    {
        _logger = logger;
        _kmsClient = new AmazonKeyManagementServiceClient(configuration.Region);
        _kmsKeyId = configuration.EncryptionKeyId;
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
}