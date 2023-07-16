using System.Reflection;
using Amazon.Lambda.Core;
using DbUp;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace update;

public class Migration
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("ConnectionString") ?? "CONNECTION_STRING_NOT_SET";

    public void FunctionHandler()
    {
        try
        {
            var upgrader =
                DeployChanges.To
                    .PostgresqlDatabase(_connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .WithVariablesDisabled()
                    .LogToConsole()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw result.Error;
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}