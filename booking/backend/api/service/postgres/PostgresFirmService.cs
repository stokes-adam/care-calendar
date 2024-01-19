using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using model.exceptions;
using model.interfaces;
using model.records;

namespace service.postgres;

public class PostgresFirmService : IFirmService
{
    private readonly ILogger<PostgresFirmService> _logger;
    private readonly IDbConnection _connection;
    
    public PostgresFirmService(ILogger<PostgresFirmService> logger, IDbConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }
    
    public async Task<Firm> GetFirm(Guid firmId)
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
        
        try
        {
            _connection.EnsureOpen();
            
            var args = new { firmId };
            
            var firm = await _connection.QueryFirstOrDefaultAsync<Firm>(sql, args)
                       ?? throw new RecordNotFoundException<Firm>(firmId);

            return firm;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get firm {FirmId}", firmId);
            throw;
        }
    }

    public async Task<Guid> CreateFirm(Firm firm)
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

        try
        {
            _connection.EnsureOpen();
            
            var args = new
            {
                ownerPersonId = firm.OwnerPersonId,
                name = firm.Name,
                address = firm.Address,
                city = firm.City
            };
            
            var id = await _connection.ExecuteScalarAsync<Guid>(sql, args);

            return id;
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