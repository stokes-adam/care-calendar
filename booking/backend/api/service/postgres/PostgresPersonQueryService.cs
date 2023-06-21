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
    
    public PostgresPersonQueryService(ILogger<PostgresPersonQueryService> logger, IConfiguration configuration, Encryption encryption)
    {
        _logger = logger;
        _connectionString = configuration.ConnectionString;
        _encryption = encryption;
    }

    public async Task<Person> GetPerson(Guid personId)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
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
            
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("personId", personId);
            
            await using var reader = command.ExecuteReader();
            
            if (!reader.Read())
            {
                throw new RecordNotFoundException<Person>(personId);
            }

            var result = new Person(
                reader.GetGuid(0),
                await _encryption.Decrypt(reader.GetString(1)),
                await _encryption.Decrypt(reader.GetString(2)),
                await _encryption.Decrypt(reader.GetString(3)),
                await _encryption.Decrypt(reader.GetString(4))
            );
            
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get person with id {PersonId}", personId);
            throw;
        }
    }
    
    public async Task<IEnumerable<Person>> GetClientsForFirm(Guid firmId)
    {
        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
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
            
            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("firmId", firmId);
            
            await using var reader = command.ExecuteReader();
            var clients = new List<Person>();
            
            while (reader.Read())
            {
                var element = new Person(
                    reader.GetGuid(0),
                    await _encryption.Decrypt(reader.GetString(1)),
                    await _encryption.Decrypt(reader.GetString(2)),
                    await _encryption.Decrypt(reader.GetString(3)),
                    await _encryption.Decrypt(reader.GetString(4))
                );
                
                clients.Add(element);
            }

            return clients;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get clients for firm with id {FirmId}", firmId);
            throw;
        }
    }

    public async Task<IEnumerable<Person>> GetConsultantsForFirm(Guid firmId)
    {
        throw new NotImplementedException();
    }
}