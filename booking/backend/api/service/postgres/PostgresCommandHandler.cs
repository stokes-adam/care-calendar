using System.Data;
using Microsoft.Extensions.Options;
using model.interfaces;
using model.records;
using Npgsql;
using Dapper;

namespace service.postgres;

public class PostgresCommandHandler(IDbConnection connection) : ICommandHandler
{
    public Task RegisterFirm(Firm firm)
    {
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

        var id = connection.ExecuteScalar<Guid>(sql, new
        {
            ownerPersonId = firm.OwnerPersonId,
            name = firm.Name
        });
        
        connection.Close();
        
        return Task.FromResult(firm with { Id = id });
    }
}