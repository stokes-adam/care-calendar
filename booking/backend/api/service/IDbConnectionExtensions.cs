using System.Data;

namespace service;

public static class DbConnectionExtensions
{
    public static void EnsureOpen(this IDbConnection connection)
    {
        if (connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }
    }
}