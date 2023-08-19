using System.Text.Json;
using Pulumi;
using Pulumi.Aws.Rds;
using Pulumi.Aws.SecretsManager;

namespace infra;

public class DatabaseComponent
{
    private Instance PostgresDb { get; }
    public DatabaseComponent(StackReference coreStack, CustomResourceOptions customResourceOptions)
    {
        var subnet1Id = coreStack.RequireOutput("subnet1Id").Apply(id => id.ToString());
        var subnet2Id = coreStack.RequireOutput("subnet2Id").Apply(id => id.ToString());
        var securityGroupId = coreStack.RequireOutput("securityGroupId").Apply(id => id.ToString());
        
        var dbSubnet = new SubnetGroup("databaseSubnet", new SubnetGroupArgs
        {
            Name = "care-calendar-db-subnet",
            SubnetIds = { subnet1Id, subnet2Id },
        }, customResourceOptions);
        
        var key = new Pulumi.Aws.Kms.Key("key", new Pulumi.Aws.Kms.KeyArgs
        {
            Description = "Key for encrypting RDS data",
        }, customResourceOptions);
        
        var secret = Output.Create(GetSecretVersion.InvokeAsync(new GetSecretVersionArgs
        {
            SecretId = "prod/CareCalendar/postgres",
        }, new InvokeOptions { Provider = customResourceOptions.Provider }));

        var getSecretString = new Func<string, Output<string>>(propertyName =>
            secret.Apply(s => JsonDocument.Parse(s.SecretString).RootElement.GetProperty(propertyName).GetString())!);
        
        var getSecretInt = new Func<string, Output<int>>(propertyName =>
            secret.Apply(s => JsonDocument.Parse(s.SecretString).RootElement.GetProperty(propertyName).GetInt32())!);
        
        PostgresDb = new Instance("postgresDb", new InstanceArgs
        {
            Identifier = getSecretString("dbInstanceIdentifier"),
            AllocatedStorage = 10,
            Engine = getSecretString("engine"),
            EngineVersion = "15.3",
            InstanceClass = "db.t3.micro",
            DbName = getSecretString("dbname"),
            Username = getSecretString("username"),
            Password = getSecretString("password"),
            Port = getSecretInt("port"),
            SkipFinalSnapshot = true,
            VpcSecurityGroupIds = { securityGroupId },
            DbSubnetGroupName = dbSubnet.Name,
        }, customResourceOptions);
        
        var connectionStringParameter = new Pulumi.Aws.Ssm.Parameter("connectionString", new Pulumi.Aws.Ssm.ParameterArgs
        {
            Type = "SecureString",
            Name = "/care-calendar/db-connection-string",
            Value = ConnectionString,
            Description = "The connection string for the PostgreSQL database",
        }, customResourceOptions);
    }
    
    public Output<string> ConnectionString => Output.Tuple(PostgresDb.Address, PostgresDb.Port, PostgresDb.DbName, PostgresDb.Username, PostgresDb.Password).Apply(t =>
    {
        var (address, port, dbName, username, password) = t;

        return $"Host={address};Port={port};Database={dbName};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    });
}