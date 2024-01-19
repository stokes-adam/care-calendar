using model.interfaces;

namespace service.postgres;

public class LocalEncryption : IEncryption
{
    public Task<string> Encrypt(string value)
    {
        return Task.FromResult(value);
    }

    public Task<string> Decrypt(string value)
    {
        return Task.FromResult(value);
    }
}