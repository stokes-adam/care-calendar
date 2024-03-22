using model.interfaces;

namespace service;

public class NoEncryption : IEncryption
{
    public Task<string> Encrypt(string value) => Task.FromResult(value);

    public Task<string> Decrypt(string value) => Task.FromResult(value);
}