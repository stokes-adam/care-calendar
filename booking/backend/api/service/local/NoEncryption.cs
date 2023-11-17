using model.interfaces;

namespace service.postgres;

public class NoEncryption : IEncryption
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