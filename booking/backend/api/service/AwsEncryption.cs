using System.Security.Cryptography;
using System.Text;
using Amazon;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Microsoft.Extensions.Options;
using model.interfaces;
using service.options;

namespace service;

public class AwsEncryption(IAmazonKeyManagementService kmsClient, IOptions<KmsKeys> keys) : IEncryption
{
    public async Task<string> Encrypt(string value)
    {
        var encryptionRequest = new EncryptRequest
        {
            KeyId = keys.Value.EncryptionKeyId,
            Plaintext = new MemoryStream(Encoding.UTF8.GetBytes(value))
        };

        var encryptionResponse = await kmsClient.EncryptAsync(encryptionRequest);

        var result = Convert.ToBase64String(encryptionResponse.CiphertextBlob.ToArray());

        return result;
    }

    public async Task<string> Decrypt(string value)
    {
        var decryptionRequest = new DecryptRequest
        {
            CiphertextBlob = new MemoryStream(Convert.FromBase64String(value))
        };

        var decryptionResponse = await kmsClient.DecryptAsync(decryptionRequest);

        var result = Encoding.UTF8.GetString(decryptionResponse.Plaintext.ToArray());

        return result;
    }
}