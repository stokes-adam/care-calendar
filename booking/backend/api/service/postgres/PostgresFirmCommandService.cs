using Microsoft.Extensions.Logging;
using model.interfaces;
using model.records;
using Npgsql;

namespace service.postgres;

public class PostgresFirmCommandService : IFirmCommandService
{
    private readonly ILogger<PostgresFirmCommandService> _logger;
    private readonly string _connectionString;
    
    public PostgresFirmCommandService(ILogger<PostgresFirmCommandService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.ConnectionString;
    }

    public Task<Firm> CreateFirm(Firm firm)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
                INSERT INTO firms (
                    owner_person_id,
                    name
                )
                VALUES (
                    @ownerPersonId,
                    @name
                )
                RETURNING id
              ";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("ownerPersonId", firm.OwnerPersonId);
            command.Parameters.AddWithValue("name", firm.Name);

            var firmId = (Guid)(command.ExecuteScalar() ?? throw new Exception("Failed to create firm"));

            return Task.FromResult(firm with { Id = firmId });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create firm");
            throw;
        }
    }

    public Task AssignFirmRoleToPerson(Guid firmId, Guid personId, string role)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            
            const string sql = @"
                INSERT INTO firms_persons_roles (
                    firm_id,
                    person_id,
                    role_id
                )
                SELECT @firmId, @personId, id
                FROM roles
                WHERE name = @role
              ";
            
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("firmId", firmId);
            command.Parameters.AddWithValue("personId", personId);
            command.Parameters.AddWithValue("role", role);
            
            command.ExecuteNonQuery();
            
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to assign role to person");
            throw;
        }
    }

    public Task RemoveFirmRoleFromPerson(Guid firmId, Guid personId, string role)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            
            const string sql = @"
                DELETE FROM firms_persons_roles
                WHERE firm_id = @firmId
                  AND person_id = @personId
                  AND role_id = (SELECT id FROM roles WHERE name = @role)
              ";
            
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("firmId", firmId);
            command.Parameters.AddWithValue("personId", personId);
            command.Parameters.AddWithValue("role", role);
            
            command.ExecuteNonQuery();
            
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to remove role from person");
            throw;
        }
    }
}