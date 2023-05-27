namespace service;

public class PostgresDatabaseConfiguration : IDatabaseConfiguration
{
    public string ConnectionString { get; }
    
    public PostgresDatabaseConfiguration(string connectionString)
    {
        ConnectionString = connectionString;
    }
}