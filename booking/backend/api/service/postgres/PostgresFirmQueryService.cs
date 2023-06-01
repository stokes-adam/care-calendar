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
    
    public PostgresFirmQueryService(ILogger<PostgresFirmQueryService> logger, IDbConfiguration dbDbConfiguration)
    {
        _logger = logger;
        _connectionString = dbDbConfiguration.ConnectionString;
    }

    public Task<Firm> GetFirm(Guid firmId)
    {
        throw new NotImplementedException();
    }
}