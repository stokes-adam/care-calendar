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
    
    var vpc = new Vpc("vpc", new VpcArgs
    {
        CidrBlock = "10.0.0.0/16",
        EnableDnsHostnames = true,
        EnableDnsSupport = true,
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

    var cluster = new Cluster("cluster", new(), customResourceOptions);


    var ghcrCredentials = Output.Create(GetSecret.InvokeAsync(new()
    {
        Name = "GHCR_Credentials"
    }, new()
    {
        Provider = provider
    }));
    
    var taskDefinition = new TaskDefinition("taskDefinition", new TaskDefinitionArgs
    {
        Family = "hello-world",
        Cpu = "256",
        Memory = "512",
        NetworkMode = "awsvpc",
        RequiresCompatibilities = {"FARGATE"},
        ExecutionRoleArn = executionRole.Arn,
        TaskRoleArn = taskRole.Arn,
        ContainerDefinitions = @"[{
        ""name"": ""hello-world"",
        ""image"": ""ghcr.io/stokes-adam/care-calendar/backend-api:latest"",
        ""portMappings"": [{
            ""containerPort"": 5057,
            ""hostPort"": 5057,
            ""protocol"": ""tcp""
        }],
        ""cpu"": 256,
        ""memory"": 512,
        ""essential"": true,
        ""repositoryCredentials"": {
            ""credentialsParameter"": """ + ghcrCredentials.Apply(c => c.Arn) + @"""
        }
    }]"
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
