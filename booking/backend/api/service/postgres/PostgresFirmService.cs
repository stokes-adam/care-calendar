using Dapper;
using Microsoft.Extensions.Logging;
using model.interfaces;
using model.records;
using Npgsql;

namespace service.postgres;

public class PostgresFirmService : IFirmService
{
    private readonly ILogger<PostgresFirmService> _logger;
    private readonly NpgsqlConnection connection;
    
    public PostgresFirmService(ILogger<PostgresFirmService> logger, IConfiguration configuration)
    {
        _logger = logger;
        connection = new NpgsqlConnection(configuration.ConnectionString);
        connection.Open();
    }
    
    public async Task<Firm> GetFirm(Guid firmId)
    {
        try
        {
            const string sql = @"
                 SELECT
                      id,
                      owner_person_id,
                      name
                 FROM firms
                 WHERE id = @firmId
                ";

            var args = new { firmId };
            
            var firm = await connection.QueryFirstAsync<Firm>(sql, args);
            
            if (firm == null)
            {
                throw new RecordNotFoundException<Firm>(firmId);
            }

            return firm;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get firm {FirmId}", firmId);
            throw;
        }
    }

    public async Task<Firm> CreateFirm(Firm firm)
    {
        try
        {
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

            var args = new
            {
                ownerPersonId = firm.OwnerPersonId,
                name = firm.Name
            };
            
            var firmId = await connection.ExecuteScalarAsync<Guid>(sql, args);
            
            return firm with { Id = firmId };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create firm");
            throw;
        }
    }

    public Task AssignFirmRoleToPerson(Guid firmId, Guid personId, string role)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFirmRoleFromPerson(Guid firmId, Guid personId, string role)
    {
        throw new NotImplementedException();
    }
}