namespace model.interfaces;

public interface IEncryption
{
    Task<string> Encrypt(string value);
    Task<string> Decrypt(string value);
}