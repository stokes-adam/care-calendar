using model;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace service;

public class PostgresFirmQueryService : IFirmQueryService
{
    private readonly ILogger<PostgresFirmQueryService> _logger;
    private readonly string _connectionString;
    private readonly string _encryptionKey;
    
    public PostgresFirmQueryService(ILogger<PostgresFirmQueryService> logger, IDatabaseConfiguration databaseConfiguration)
    {
        _logger = logger;
        _connectionString = databaseConfiguration.ConnectionString;
        _encryptionKey = databaseConfiguration.EncryptionKey;
    }

    public Task<IEnumerable<Firm>> GetForOwnerPersonWithEmail(string email)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
                SELECT 
                    f.id,
                    f.owner_person_id,
                    f.name
                FROM person_details pd
                JOIN persons p ON pd.person_id = p.id
                JOIN firms f ON p.id = f.owner_person_id
                WHERE pd.email = @email
                AND f.deleted = false
                AND p.deleted = false
                AND pd.deleted = false
            ";
            
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("email", email);
            
            using var reader = command.ExecuteReader();
            var clients = new List<Firm>();
            
            while (reader.Read())
            {
                clients.Add(new Firm(
                    reader.GetGuid(0),
                    reader.GetGuid(1),
                    reader.GetString(2)
                ));
            }
            
            return Task.FromResult(clients.AsEnumerable());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting firms for person with email {Email}", email);
            throw;
        }
    }
}