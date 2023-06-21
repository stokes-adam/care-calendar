using Microsoft.Extensions.Logging;
using model;
using model.interfaces;
using model.records;
using Npgsql;

namespace service.postgres;

public class PostgresFirmQueryService : IFirmQueryService
{
    private readonly ILogger<PostgresFirmQueryService> _logger;
    private readonly string _connectionString;

    public PostgresFirmQueryService(ILogger<PostgresFirmQueryService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.ConnectionString;
    }

    public Task<Firm> GetFirm(Guid firmId)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = @"
                 SELECT
                      id,
                      owner_person_id,
                      name
                 FROM firms
                 WHERE id = @firmId
                ";

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("firmId", firmId);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
            {
                throw new RecordNotFoundException<Firm>(firmId);
            }

            var firm = new Firm(
                reader.GetGuid(0),
                reader.GetGuid(1),
                reader.GetString(2));

            return Task.FromResult(firm);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get firm {FirmId}", firmId);
            throw;
        }
    }
}