using Dapper;
using Microsoft.Extensions.Logging;
using model.exceptions;
using model.interfaces;
using model.records;
using Npgsql;

namespace service.postgres;

public class PostgresFirmService : IFirmService
{
    private readonly ILogger<PostgresFirmService> _logger;
    private readonly NpgsqlConnection _connection;
    
    public PostgresFirmService(ILogger<PostgresFirmService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connection = new NpgsqlConnection(configuration.ConnectionString);
        _connection.Open();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
    
    public async Task<Firm> GetFirm(Guid firmId)
    {
        try
        {
            const string sql = @"
                 SELECT
                    id,
                    created,
                    updated,
                    deleted,
                    owner_person_id,
                    name,
                    address,
                    city
                 FROM firms
                 WHERE id = @firmId
                ";

            var args = new { firmId };
            
            var firm = await _connection.QueryFirstAsync<Firm>(sql, args);
            
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
                    name,
                    address,
                    city
                )
                VALUES (
                    @ownerPersonId,
                    @name,
                    @address,
                    @city
                )
                RETURNING id
              ";

            var args = new
            {
                ownerPersonId = firm.OwnerPersonId,
                name = firm.Name,
                address = firm.Address,
                city = firm.City
            };
            
            var firmId = await _connection.ExecuteScalarAsync<Guid>(sql, args);
            
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