namespace model.interfaces;

public interface IDbConfiguration
{
    string ConnectionString { get; }
    string EncryptionKey { get; }
}