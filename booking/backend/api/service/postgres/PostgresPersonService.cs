using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using model.dtos;
using model.interfaces;

namespace service.postgres;

public class PostgresPersonService : IPersonService
{
    private readonly ILogger<PostgresPersonService> _logger;
    private readonly IDbConnection _connection;
    private readonly IEncryption _encryption;
    
    public PostgresPersonService(ILogger<PostgresPersonService> logger, IDbConnection connection, IEncryption encryption)
    {
        _logger = logger;
        _encryption = encryption;
        _connection = connection;
    }
    
    public async Task<Guid> CreatePerson()
    {
        const string sql = @"
            INSERT INTO persons DEFAULT VALUES
            RETURNING id
            ";
        
        try
        {
            _connection.EnsureOpen();
        
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
        
        try
        {
            _connection.EnsureOpen();

            var args = new
            {
                personId,
                encryptedFirstName = await _encryption.Encrypt(personDetail.FirstName),
                encryptedLastName = await _encryption.Encrypt(personDetail.LastName),
                encryptedEmail = await _encryption.Encrypt(personDetail.Email),
                encryptedPhone = await _encryption.Encrypt(personDetail.Phone)
            };
            
            var id = await _connection.QueryFirstAsync<Guid>(sql, args);

            return id;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create person detail" );
            throw;
        }
    }
}