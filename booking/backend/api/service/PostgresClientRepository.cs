using System.Data.Common;
using model;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace service.Classes;

public class PostgresClientRepository : IClientRepository
{
    private readonly ILogger<PostgresClientRepository> _logger;
    private readonly IApiConfig _apiConfig;
    
    public PostgresClientRepository(ILogger<PostgresClientRepository> logger, IApiConfig apiConfig)
    {
        _logger = logger;
        _apiConfig = apiConfig;
    }

    public Task<IEnumerable<Client>> GetClientsForFirm(Guid firmId)
    {
        try
        {
            using var connection = new NpgsqlConnection(_apiConfig.ConnectionString);
            connection.Open();
            
            const string sql = @"
                SELECT
                    id,
                    person_id,
                    firm_id,
                    active
                FROM clients
                WHERE
                    firm_id = @firmId";
            
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
            _logger.LogError(e, "Error getting clients for firm {firmId}", firmId);
            throw;
        }
    }

    public Task<Client> GetClient(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Client> AddClient(Client client)
    {
        throw new NotImplementedException();
    }

    public Task<Client> UpdateClient(Client client)
    {
        throw new NotImplementedException();
    }

    public Task<Client> DeleteClient(Guid id)
    {
        throw new NotImplementedException();
    }
}