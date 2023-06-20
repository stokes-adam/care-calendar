using Pulumi;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;
using Pulumi.Aws.Ecs;
using Pulumi.Aws.Ecs.Inputs;
using Pulumi.Aws.Iam;

return await Deployment.RunAsync(() =>
{
    var vpc = new Vpc("vpc", new VpcArgs
    {
        CidrBlock = "10.0.0.0/16",
        EnableDnsHostnames = true,
        EnableDnsSupport = true,
    });
    
    var subnet1 = new Subnet("subnet1", new SubnetArgs
    {
        VpcId = vpc.Id,
        CidrBlock = "10.0.1.0/24",
        AvailabilityZone = "us-east-1a",
    });
    
    var subnet2 = new Subnet("subnet2", new SubnetArgs
    {
        VpcId = vpc.Id,
        CidrBlock = "10.0.2.0/24",
        AvailabilityZone = "us-east-1b",
    });

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
        }
    });


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
    });
    
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
    });
    
    var cluster = new Cluster("cluster");
    
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
        ""image"": ""ghcr.io/stokes-adam/backend-api:latest"",
        ""portMappings"": [{
            ""containerPort"": 80,
            ""hostPort"": 80,
            ""protocol"": ""tcp""
        }],
        ""cpu"": 256,
        ""memory"": 512,
        ""essential"": true
    }]"
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
            Subnets = { subnet1.Id , subnet2.Id },
            SecurityGroups = { securityGroup.Id }
        }
    });
});
