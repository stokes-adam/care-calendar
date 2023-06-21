using Microsoft.Extensions.Logging;
using model.interfaces;
using model.records;
using Npgsql;

namespace service.postgres;

public class PostgresPersonCommandService : IPersonCommandService
{
    private readonly ILogger<PostgresPersonCommandService> _logger;
    private readonly string _connectionString;
    private readonly Encryption _encryption;
    
    public PostgresPersonCommandService(ILogger<PostgresPersonCommandService> logger, IConfiguration configuration, Encryption encryption)
    {
        _logger = logger;
        _connectionString = configuration.ConnectionString;
        _encryption = encryption;
    }
    
    public Task<Person> CreatePerson(Person person)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            
            // first add record to person
            // then add record to person_details using id from person

            const string addPersonSql = @"
                INSERT INTO persons DEFAULT VALUES RETURNING id;
            ";
            
            using var addPersonCommand = new NpgsqlCommand(addPersonSql, connection);
            var personId = (Guid) (addPersonCommand.ExecuteScalar() ?? throw new Exception("Failed to create person"));
            
            const string addDetailsSql = @"
                INSERT INTO persons_details (person_id, encrypted_first_name, encrypted_last_name, encrypted_email, encrypted_phone)
                VALUES (@personId, @encryptedFirstName, @encryptedLastName, @encryptedEmail, @encryptedPhone);
            ";
            
            using var addDetailsCommand = new NpgsqlCommand(addDetailsSql, connection);
            addDetailsCommand.Parameters.AddWithValue("personId", personId);
            addDetailsCommand.Parameters.AddWithValue("encryptedFirstName", _encryption.Encrypt(person.FirstName));
            addDetailsCommand.Parameters.AddWithValue("encryptedLastName", _encryption.Encrypt(person.LastName));
            addDetailsCommand.Parameters.AddWithValue("encryptedEmail", _encryption.Encrypt(person.Email));
            addDetailsCommand.Parameters.AddWithValue("encryptedPhone", _encryption.Encrypt(person.Phone));
            
            addDetailsCommand.ExecuteNonQuery();
            
            return Task.FromResult(person with { Id = personId });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create person");
            throw;
        }
    }

    public Task<Person> UpdatePerson(Person person)
    {
        throw new NotImplementedException();
    }

    public Task DeletePerson(Guid personId)
    {
        throw new NotImplementedException();
    }

    public Task DeletePersonalDetails(Guid personId)
    {
        throw new NotImplementedException();
    }
}