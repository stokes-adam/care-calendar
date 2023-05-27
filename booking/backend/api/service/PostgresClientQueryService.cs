using model;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace service;

public class PostgresClientQueryService : IClientQueryService
{
    private readonly ILogger<PostgresClientQueryService> _logger;
    private readonly IDatabaseConfiguration _databaseConfiguration;
    
    public PostgresClientQueryService(ILogger<PostgresClientQueryService> logger, IDatabaseConfiguration databaseConfiguration)
    {
        _logger = logger;
        _databaseConfiguration = databaseConfiguration;
    }

    public Task<IEnumerable<Client>> GetClientsForFirm(Guid firmId)
    {
        try
        {
            using var connection = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
            connection.Open();
            
            const string sql = @"
                SELECT
                    id,
                    person_id,
                    firm_id
                FROM clients
                WHERE firm_id = @firmId
                AND deleted = false";
            
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("firmId", firmId);
            
            using var reader = command.ExecuteReader();
            var clients = new List<Client>();
            
            while (reader.Read())
            {
                clients.Add(new Client(
                    reader.GetGuid(0),
                    reader.GetGuid(1),
                    reader.GetGuid(2),
                    reader.GetBoolean(3)
                ));
            }
            
            return Task.FromResult(clients.AsEnumerable());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting clients for firm {FirmId}", firmId);
            throw;
        }
    }

    public Task<Client> GetClient(Guid id)
    {
        try
        {
            using var connection = new NpgsqlConnection(_databaseConfiguration.ConnectionString);
            connection.Open();
            
            const string sql = @"
                SELECT
                    id,
                    person_id,
                    firm_id
                FROM clients
                WHERE id = @id
                AND deleted = false";
            
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", id);
            
            using var reader = command.ExecuteReader();
            
            if (!reader.Read())
            {
                throw new RecordNotFoundException<Client>(id);
            }
            
            return Task.FromResult(new Client(
                reader.GetGuid(0),
                reader.GetGuid(1),
                reader.GetGuid(2),
                false
            ));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting client {Id}", id);
            throw;
        }
    }
}