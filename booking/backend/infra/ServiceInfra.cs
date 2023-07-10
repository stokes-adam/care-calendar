using System.Text.Json;
using Pulumi;
using Pulumi.Aws.Ecs;
using Pulumi.Aws.Ecs.Inputs;
using Pulumi.Aws.Iam;
using Pulumi.Aws.SecretsManager;

namespace infra;

public class ServiceInfra
{
    public ServiceInfra(NetworkInfra networkInfra, CustomResourceOptions customResourceOptions)
    {
        var runNumber = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER") ?? "INFRA-GITHUB_RUN_NUMBER-NOT-SET";

        var invokeOptions = new InvokeOptions() { Provider = customResourceOptions.Provider };
        
        var ghcrCredentials = Output.Create(GetSecret.InvokeAsync(new()
        {
            Name = "GHCR_Credentials"
        }, invokeOptions));

        var logGroup = new Pulumi.Aws.CloudWatch.LogGroup("logGroup", new Pulumi.Aws.CloudWatch.LogGroupArgs
        {
            Name = "care-calendar",
            RetentionInDays = 7,
        }, customResourceOptions);

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
        }, customResourceOptions);

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
        }, customResourceOptions);

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
        }, customResourceOptions);

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
        }, customResourceOptions);

        var cluster = new Cluster("cluster", new(), customResourceOptions);
        
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
        }, customResourceOptions);

        var service = new Service("service", new ServiceArgs
        {
            Cluster = cluster.Arn,
            DesiredCount = 1,
            LaunchType = "FARGATE",
            TaskDefinition = taskDefinition.Arn,
            NetworkConfiguration = new ServiceNetworkConfigurationArgs
            {
                AssignPublicIp = true,
                Subnets = { networkInfra.Subnet1Id, networkInfra.Subnet2Id },
                SecurityGroups = { networkInfra.SecurityGroupId }
            }
        }, customResourceOptions);
    }
}