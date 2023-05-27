namespace service;

public class PostgresDatabaseConfiguration : IDatabaseConfiguration
{
    public string ConnectionString { get; }
    public string EncryptionKey { get; }
    
    public PostgresDatabaseConfiguration(string connectionString, string encryptionKey)
    {
        ConnectionString = connectionString;
        EncryptionKey = encryptionKey;
    }
}