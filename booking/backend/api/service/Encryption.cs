using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using model.interfaces;

namespace service;
public class Encryption
{
    private readonly ILogger<Encryption> _logger;
    private readonly byte[] _encryptionKey;

    public Encryption(ILogger<Encryption> logger, IDbConfiguration dbConfiguration)
    {
        _logger = logger;
        _encryptionKey = Encoding.UTF8.GetBytes(dbConfiguration.EncryptionKey);
    }

    public string Encrypt(string value)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.GenerateIV();

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var clearBytes = Encoding.UTF8.GetBytes(value);
            var encrypted = encryptor.TransformFinalBlock(clearBytes, 0, clearBytes.Length);

            var encryptedWithIv = aes.IV.Concat(encrypted).ToArray();
            
            var base64 = Convert.ToBase64String(encryptedWithIv);

            return base64;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string Decrypt(string value)
    {
        try
        {
            var fullCipher = Convert.FromBase64String(value);

            var iv = new byte[16];
            var cipherText = new byte[fullCipher.Length - 16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);

            using var aes = Aes.Create();
            aes.Key = _encryptionKey;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decrypted = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return Encoding.UTF8.GetString(decrypted);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}