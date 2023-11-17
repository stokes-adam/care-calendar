using Dapper;
using Microsoft.Extensions.Logging;
using model.dtos;
using model.interfaces;
using Npgsql;

namespace service.postgres;

public class PostgresPersonService : IPersonService
{
    private readonly ILogger<PostgresPersonService> _logger;
    private readonly NpgsqlConnection _connection;
    private readonly IEncryption _encryption;
    
    public PostgresPersonService(ILogger<PostgresPersonService> logger, IConfiguration configuration, IEncryption encryption)
    {
        _logger = logger;
        _encryption = encryption;
        _connection = new NpgsqlConnection(configuration.ConnectionString);
        _connection.Open();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
    
    public async Task<Guid> CreatePerson()
    {
        try
        {
            const string sql = @"
                INSERT INTO persons DEFAULT VALUES
                RETURNING id
                ";

            var id = await _connection.QueryFirstAsync<Guid>(sql);

            return id;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create person");
            throw;
        }
    }

    public async Task<Guid> CreatePersonDetail(Guid personId, CreatePersonDetail personDetail)
    {
        try
        {
            const string sql = @"
                INSERT INTO person_details (
                    person_id,
                    encrypted_first_name,
                    encrypted_last_name,
                    encrypted_email,
                    encrypted_phone
                )
                VALUES (
                    @personId,
                    @encryptedFirstName,
                    @encryptedLastName,
                    @encryptedEmail,
                    @encryptedPhone
                )
                RETURNING id
                ";
            
            var id = await _connection.QueryFirstAsync<Guid>(sql, new
            {
                personId,
                encryptedFirstName = await _encryption.Encrypt(personDetail.FirstName),
                encryptedLastName = await _encryption.Encrypt(personDetail.LastName),
                encryptedEmail = await _encryption.Encrypt(personDetail.Email),
                encryptedPhone = await _encryption.Encrypt(personDetail.Phone)
            });
            
            return id;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create person detail" );
            throw;
        }
    }
}