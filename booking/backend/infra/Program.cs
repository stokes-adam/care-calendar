using System.Globalization;
using System.Text.Json;
using infra;
using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Ecs;
using Pulumi.Aws.Ecs.Inputs;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.Lambda.Inputs;
using Pulumi.Aws.Rds;
using Pulumi.Aws.SecretsManager;
using Cluster = Pulumi.Aws.Rds.Cluster;

return await Deployment.RunAsync(() =>
{
    var runNumber = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER") ?? "INFRA-GITHUB_RUN_NUMBER-NOT-SET";
    
    var core = new StackReference("care-calendar/infra-core/default");

    var subnet1Id = core.Output<string>("subnet1Id");
    var subnet2Id = core.Output<string>("subnet2Id");
    var securityGroupId = core.Output<string>("securityGroupId");
    var region = core.Output<string>("providerRegion");
    var targetGroupArn = core.Output<string>("targetGroupArn");
    
    var provider = new Provider("us-east-1", new() { Region = region });
    
    var dependsOnCore = new CustomResourceOptions
    {
        DependsOn = core,
        Provider = provider
    };
    
    var secret = Output.Create(GetSecretVersion.InvokeAsync(new GetSecretVersionArgs
    {
        SecretId = "prod/CareCalendar/postgres",
    }, new ()
    {
        Provider = provider
    }));
    
    var ghcrCredentials = Output.Create(GetSecret.InvokeAsync(new()
    {
        Name = "GHCR_Credentials"
    }, new ()
    {
        Provider = provider
    }));

    var secretProperties = new SecretProperties(secret);
    
    var key = new Pulumi.Aws.Kms.Key("key", new Pulumi.Aws.Kms.KeyArgs
    {
        Description = "Key for encrypting RDS data",
    }, dependsOnCore);

    var dbSubnet = new SubnetGroup("databaseSubnet", new SubnetGroupArgs
    {
        Name = "care-calendar-database-subnet",
        SubnetIds = { subnet1Id, subnet2Id },
    }, new ()
    {
        DependsOn = core,
        Provider = provider
    });

    var postgresDb = new Instance("postgresDb", new InstanceArgs
    {
        Identifier = secretProperties.GetString("dbInstanceIdentifier"),
        AllocatedStorage = 10,
        Engine = secretProperties.GetString("engine"),
        EngineVersion = "15.3",
        InstanceClass = "db.t3.micro",
        DbName = secretProperties.GetString("dbname"),
        Username = secretProperties.GetString("username"),
        Password = secretProperties.GetString("password"),
        Port = secretProperties.GetInt("port"),
        SkipFinalSnapshot = true,
        VpcSecurityGroupIds = { securityGroupId },
        DbSubnetGroupName = dbSubnet.Name,
    }, dependsOnCore);

    var connectionString = BuildConnectionString(postgresDb);

    // we create a parameter for the api because it has a docker container which can be pulled.
    // the migration tool is AWS lambda and so cannot be accessed by contributers to get the connection string
    // from the environment variables.
    // the api can be accessed by contributers to get the connection string from the environment variables.
    var connectionStringParameter = new Pulumi.Aws.Ssm.Parameter("connectionString", new Pulumi.Aws.Ssm.ParameterArgs
    {
        Type = "SecureString",
        Name = "/care-calendar/db-connection-string",
        Value = connectionString,
        Description = "The connection string for the PostgreSQL database",
    }, dependsOnCore);
    
    var migrationRole = new Role("migrationRole", new RoleArgs
    {
        AssumeRolePolicy = @"{
        ""Version"": ""2012-10-17"",
        ""Statement"": [
            {
            ""Action"": ""sts:AssumeRole"",
            ""Principal"": {
                ""Service"": ""lambda.amazonaws.com""
            },
            ""Effect"": ""Allow"",
            ""Sid"": """"
            }
        ]
        }"
    }, dependsOnCore);

    var migrationRolePolicyAttachment = new RolePolicyAttachment("migrationRolePolicyAttachment",
        new RolePolicyAttachmentArgs
        {
            Role = migrationRole.Name,
            PolicyArn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
        }, dependsOnCore);

    var migrationLambda = new Function("migrationLambda", new FunctionArgs
    {
        Code = new FileArchive("./update"),
        Role = migrationRole.Arn,
        Handler = "update::update.Migration::FunctionHandler",
        Runtime = Runtime.Dotnet6,
        Timeout = 60,
        VpcConfig = new FunctionVpcConfigArgs
        {
            SubnetIds = { subnet1Id, subnet2Id },
            SecurityGroupIds = { securityGroupId }
        },
        Environment = new FunctionEnvironmentArgs
        {
            Variables =
            {
                { "ConnectionString", connectionString },
            }
        },
    }, new ()
    {
        DependsOn = postgresDb,
        Provider = provider
    });

    var migrationInvocation = new Invocation("migration", new()
    {
        FunctionName = migrationLambda.Name,
        Input = "{}",
        Triggers =
        {
            { "redeployment", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) }
        },
    }, new CustomResourceOptions
    {
        DependsOn = { migrationLambda },
        Provider = provider,
    });
    
    var logGroup = new Pulumi.Aws.CloudWatch.LogGroup("logGroup", new Pulumi.Aws.CloudWatch.LogGroupArgs
    {
        Name = "care-calendar",
        RetentionInDays = 7,
    }, dependsOnCore);

    var taskRole = new Role("taskRole", new RoleArgs
    {
        AssumeRolePolicy = @"{
            ""Version"": ""2008-10-17"",
            ""Statement"": [{
                ""Sid"": """",
                ""Effect"": ""Allow"",
                ""Principal"": {
                    ""Service"": ""ecs-tasks.amazonaws.com""
                },
                ""Action"": ""sts:AssumeRole""
            }]
        }"
    }, dependsOnCore);

    var executionRole = new Role("executionRole", new RoleArgs
    {
        AssumeRolePolicy = @"{
            ""Version"": ""2008-10-17"",
            ""Statement"": [{
                ""Sid"": """",
                ""Effect"": ""Allow"",
                ""Principal"": {
                    ""Service"": ""ecs-tasks.amazonaws.com""
                },
                ""Action"": ""sts:AssumeRole""
            }]
        }"
    }, dependsOnCore);

    var executionRoleSecretPolicy = new RolePolicy("executionRolePolicy", new RolePolicyArgs
    {
        Role = executionRole.Name,
        Policy = ghcrCredentials.Apply(cred => JsonSerializer.Serialize(new
        {
            Version = "2012-10-17",
            Statement = new[]
            {
                new
                {
                    Action = "secretsmanager:GetSecretValue",
                    Resource = cred.Arn,
                    Effect = "Allow"
                }
            }
        })),
    }, dependsOnCore);

    var executionRoleLogPolicy = new RolePolicy("taskRolePolicy", new RolePolicyArgs
    {
        Role = executionRole.Name,
        Policy = logGroup.Arn.Apply(arn => JsonSerializer.Serialize(new
        {
            Version = "2012-10-17",
            Statement = new[]
            {
                new
                {
                    Action = "logs:CreateLogStream",
                    Resource = arn + ":log-stream:*",
                    Effect = "Allow"
                },
                new
                {
                    Action = "logs:PutLogEvents",
                    Resource = arn + ":log-stream:*",
                    Effect = "Allow"
                }
            }
        })),
    }, dependsOnCore);

    var cluster = new Cluster("cluster", new(), dependsOnCore);
    
    var taskDefinition = new TaskDefinition("taskDefinition", new TaskDefinitionArgs
    {
        Family = "hello-world",
        Cpu = "256",
        Memory = "512",
        NetworkMode = "awsvpc",
        RequiresCompatibilities = { "FARGATE" },
        ExecutionRoleArn = executionRole.Arn,
        TaskRoleArn = taskRole.Arn,
        ContainerDefinitions = ghcrCredentials.Apply(c => $@"[{{
            ""name"": ""hello-world"",
            ""image"": ""ghcr.io/stokes-adam/care-calendar/backend-api:{runNumber}"",
            ""portMappings"": [{{
                ""containerPort"": 5057,
                ""hostPort"": 5057,
                ""protocol"": ""tcp""
            }}],
            ""cpu"": 256,
            ""memory"": 512,
            ""essential"": true,
            ""logConfiguration"": {{
                ""logDriver"": ""awslogs"",
                ""options"": {{
                    ""awslogs-group"": ""care-calendar"",
                    ""awslogs-region"": ""us-east-1"",
                    ""awslogs-stream-prefix"": ""hello-world""
                }}
            }},
            ""repositoryCredentials"": {{
                ""credentialsParameter"": """ + c.Arn + @"""
            }
        }]"
        )
    }, new ()
    {
        DependsOn = migrationInvocation,
        Provider = provider
    });
    
    var service = new Service("service", new ServiceArgs
    {
        Cluster = cluster.Arn,
        DesiredCount = 1,
        LaunchType = "FARGATE",
        TaskDefinition = taskDefinition.Arn,
        NetworkConfiguration = new ServiceNetworkConfigurationArgs
        {
            AssignPublicIp = true,
            Subnets = { subnet1Id, subnet2Id },
            SecurityGroups = { securityGroupId }
        },
    }, dependsOnCore);
});

Output<string> BuildConnectionString(Instance postgresDb) => Output
    .Tuple(postgresDb.Address, postgresDb.Port, postgresDb.DbName, postgresDb.Username, postgresDb.Password).Apply(
        t =>
        {
            var (address, port, dbName, username, password) = t;

            return
                $"Host={address};Port={port};Database={dbName};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        });