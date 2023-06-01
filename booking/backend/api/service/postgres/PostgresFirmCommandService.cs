using Microsoft.Extensions.Logging;
using model.enums;
using model.interfaces;
using model.records;
using Npgsql;

namespace service.postgres;

public class PostgresFirmCommandService : IFirmCommandService
{
    private readonly ILogger<PostgresFirmCommandService> _logger;
    private readonly string _connectionString;
    
    public PostgresFirmCommandService(ILogger<PostgresFirmCommandService> logger, IDbConfiguration dbDbConfiguration)
    {
        _logger = logger;
        _connectionString = dbDbConfiguration.ConnectionString;
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

    public Task AssignRoleToPerson(Guid firmId, Guid personId, Role role)
    {
        throw new NotImplementedException();
    }
}