using Microsoft.Extensions.Logging;
using model.interfaces;
using model.records;
using Npgsql;

namespace service.postgres;

public class PostgresPersonQueryService : IPersonQueryService
{
    private readonly ILogger<PostgresPersonQueryService> _logger;
    private readonly string _connectionString;
    private readonly Encryption _encryption;
    
    public PostgresPersonQueryService(ILogger<PostgresPersonQueryService> logger, IDbConfiguration dbDbConfiguration, Encryption encryption)
    {
        _logger = logger;
        _connectionString = dbDbConfiguration.ConnectionString;
        _encryption = encryption;
    }

    public Task<Person> GetPerson(Guid personId)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            
            const string sql = @"
                SELECT
                    pd.person_id,
                    pd.encrypted_first_name,
                    pd.encrypted_last_name,
                    pd.encrypted_email,
                    pd.encrypted_phone
                FROM persons_details pd
                WHERE pd.person_id = @personId
                AND pd.deleted IS NULL
            ";
            
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("personId", personId);
            
            using var reader = command.ExecuteReader();
            
            if (!reader.Read())
            {
                throw new RecordNotFoundException<Person>(personId);
            }
            
            return Task.FromResult(new Person(
                reader.GetGuid(0),
                _encryption.Decrypt(reader.GetString(1)),
                _encryption.Decrypt(reader.GetString(2)),
                _encryption.Decrypt(reader.GetString(3)),
                _encryption.Decrypt(reader.GetString(4))
            ));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get person with id {PersonId}", personId);
            throw;
        }
    }
    
    public Task<IEnumerable<Person>> GetClientsForFirm(Guid firmId)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
                SELECT
                    pd.person_id,
                    pd.encrypted_first_name,
                    pd.encrypted_last_name,
                    pd.encrypted_email,
                    pd.encrypted_phone
                FROM persons_details pd
                JOIN firms f ON pd.person_id = f.owner_person_id
                WHERE f.id = @firmId
                AND f.deleted IS NULL
                AND pd.deleted IS NULL
            ";
            
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("firmId", firmId);
            
            using var reader = command.ExecuteReader();
            var clients = new List<Person>();
            
            while (reader.Read())
            {
                clients.Add(new Person(
                    reader.GetGuid(0),
                    _encryption.Decrypt(reader.GetString(1)),
                    _encryption.Decrypt(reader.GetString(2)),
                    _encryption.Decrypt(reader.GetString(3)),
                    _encryption.Decrypt(reader.GetString(4))
                ));
            }
            
            return Task.FromResult(clients.AsEnumerable());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get clients for firm with id {FirmId}", firmId);
            throw;
        }
    }

    public Task<IEnumerable<Person>> GetConsultantsForFirm(Guid firmId)
    {
        throw new NotImplementedException();
    }
}