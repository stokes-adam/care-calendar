using System.Text.Json;
using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;
using Pulumi.Aws.Ecs;
using Pulumi.Aws.Ecs.Inputs;
using Pulumi.Aws.Iam;
using Pulumi.Aws.SecretsManager;
using Rds = Pulumi.Aws.Rds;
using Cluster = Pulumi.Aws.Ecs.Cluster;
using SecurityGroup = Pulumi.Aws.Ec2.SecurityGroup;
using SecurityGroupArgs = Pulumi.Aws.Ec2.SecurityGroupArgs;

return await Deployment.RunAsync(() =>
{
    var runNumber = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER") ?? "INFRA-GITHUB_RUN_NUMBER-NOT-SET";
    var coreStack = new StackReference("care-calendar/infra-core/default");
    var providerRegion = coreStack.RequireOutput("providerRegion").Apply(region => region.ToString());
    
    var provider = new Provider("us-east-1", new ProviderArgs
    {
        Region = providerRegion,
    });
    
    var customResourceOptions = new CustomResourceOptions {
        Provider = provider,
        DependsOn = coreStack
    };
    
    var ghcrCredentials = Output.Create(GetSecret.InvokeAsync(new()
    {
        Name = "GHCR_Credentials"
    }, new()
    {
        Provider = provider
    }));
    
    var vpc = new Vpc("vpc", new VpcArgs
    {
        CidrBlock = "10.0.0.0/16",
        EnableDnsHostnames = true,
        EnableDnsSupport = true,
    }, customResourceOptions);
    
    var internetGateway = new InternetGateway("internetGateway", new InternetGatewayArgs
    {
    }, customResourceOptions);

    var vpcInternetGatewayAttachment = new InternetGatewayAttachment("vpcInternetGatewayAttachment", new ()
    {
        VpcId = vpc.Id,
        InternetGatewayId = internetGateway.Id,
    }, customResourceOptions); 
    
    var routeTable = new RouteTable("routeTable", new RouteTableArgs
    {
        VpcId = vpc.Id,
        Routes = 
        {
            new RouteTableRouteArgs
            {
                CidrBlock = "0.0.0.0/0",
                GatewayId = internetGateway.Id,
            }
        },
    }, customResourceOptions);
 
    
    var subnet1 = new Subnet("subnet1", new SubnetArgs
    {
        VpcId = vpc.Id,
        CidrBlock = "10.0.1.0/24",
    }, customResourceOptions);
    
    var subnet2 = new Subnet("subnet2", new SubnetArgs
    {
        VpcId = vpc.Id,
        CidrBlock = "10.0.2.0/24",
    }, customResourceOptions);
    
    var routeTableAssociation1 = new RouteTableAssociation("routeTableAssociation1", new RouteTableAssociationArgs
    {
        SubnetId = subnet1.Id,
        RouteTableId = routeTable.Id,
    }, customResourceOptions);

    var routeTableAssociation2 = new RouteTableAssociation("routeTableAssociation2", new RouteTableAssociationArgs
    {
        SubnetId = subnet2.Id,
        RouteTableId = routeTable.Id,
    }, customResourceOptions); 

    var securityGroup = new SecurityGroup("securityGroup", new SecurityGroupArgs
    {
        VpcId = vpc.Id,
        Ingress =
        {
            new SecurityGroupIngressArgs
            {
                Protocol = "tcp",
                FromPort = 80,
                ToPort = 80,
                CidrBlocks = {"0.0.0.0/0"},
            }
        },
        Egress =
        {
            new SecurityGroupEgressArgs
            {
                Protocol = "-1",
                FromPort = 0,
                ToPort = 0,
                CidrBlocks = {"0.0.0.0/0"}
            }
        }
    }, customResourceOptions);
    
    var logGroup = new Pulumi.Aws.CloudWatch.LogGroup("logGroup", new Pulumi.Aws.CloudWatch.LogGroupArgs
    {
        Name = "care-calendar",
        RetentionInDays = 7,
    }, customResourceOptions);


    // task role needs RDS permission
    // aws secrets manager permission
    // cloudwatch logs permission
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
    
    var taskRolePolicy = new RolePolicy("taskRolePolicy", new RolePolicyArgs
    {
        Role = taskRole.Name,
        Policy = logGroup.Arn.Apply(arn => JsonSerializer.Serialize(new
        {
            Version = "2012-10-17",
            Statement = new[]
            {
                new 
                {
                    Action = "logs:CreateLogStream",
                    Resource = arn,
                    Effect = "Allow"
                },
                new 
                {
                    Action = "logs:PutLogEvents",
                    Resource = arn,
                    Effect = "Allow"
                }
            }
        })),
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
    
    var executionRolePolicy = new RolePolicy("executionRolePolicy", new RolePolicyArgs
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

    var cluster = new Cluster("cluster", new(), customResourceOptions);


    var taskDefinition = new TaskDefinition("taskDefinition", new TaskDefinitionArgs
    {
        Family = "hello-world",
        Cpu = "256",
        Memory = "512",
        NetworkMode = "awsvpc",
        RequiresCompatibilities = {"FARGATE"},
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
            Subnets = { subnet1.Id , subnet2.Id },
            SecurityGroups = { securityGroup.Id }
        }
    }, customResourceOptions);
});
