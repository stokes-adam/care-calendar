using model.interfaces;

namespace service;

public class DbConfiguration : IDbConfiguration
{
    public string ConnectionString { get; }
    public string EncryptionKey { get; }
    
    public DbConfiguration(string connectionString, string encryptionKey)
    {
        ConnectionString = connectionString;
        EncryptionKey = encryptionKey;
    }
}