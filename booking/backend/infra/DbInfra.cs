using Pulumi;
using Pulumi.Aws.Rds;

namespace infra;

public class DbInfra
{
    private Instance PostgresDb { get; }
    public DbInfra(NetworkInfra networkInfra, CustomResourceOptions customResourceOptions)
    {
        var dbSubnet = new SubnetGroup("dbSubnet", new SubnetGroupArgs
        {
            Name = "care-calendar-db-subnet",
            SubnetIds = { networkInfra.Subnet1Id, networkInfra.Subnet2Id },
        }, customResourceOptions);
        
        var key = new Pulumi.Aws.Kms.Key("key", new Pulumi.Aws.Kms.KeyArgs
        {
            Description = "Key for encrypting RDS data",
        }, customResourceOptions);

        PostgresDb = new Instance("postgresDb", new InstanceArgs
        {
            Identifier = "carecalendar",
            AllocatedStorage = 10,
            Engine = "postgres",
            EngineVersion = "15.3",
            InstanceClass = "db.t3.micro",
            Name = "carecalendar",
            ManageMasterUserPassword = true,
            MasterUserSecretKmsKeyId = key.KeyId,
            SkipFinalSnapshot = true,
            Username = "postgres",
            VpcSecurityGroupIds = { networkInfra.SecurityGroupId },
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
    
    public Output<string> ConnectionString => Output.Tuple(PostgresDb.Endpoint, PostgresDb.Username, PostgresDb.Password).Apply(t =>
    {
        var (endpoint, username, password) = t;
        
        return $"Server={endpoint};Port=5432;Database=carecalendar;User Id={username};Password={password};";
    });
}