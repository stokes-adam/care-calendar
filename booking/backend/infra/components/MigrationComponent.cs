using System.Globalization;
using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.Lambda.Inputs;

namespace infra.components;

public class MigrationComponent
{
    public Invocation MigrationInvocation { get; }

    public MigrationComponent(StackReference core, DatabaseComponent databaseComponent, CustomResourceOptions customResourceOptions)
    {
        var subnet1Id = core.RequireOutput("subnet1Id").Apply(id => id.ToString());
        var subnet2Id = core.RequireOutput("subnet2Id").Apply(id => id.ToString());
        var securityGroupId = core.RequireOutput("securityGroupId").Apply(id => id.ToString());
        
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
        }, customResourceOptions);

        var migrationRolePolicyAttachment = new RolePolicyAttachment("migrationRolePolicyAttachment",
            new RolePolicyAttachmentArgs
            {
                Role = migrationRole.Name,
                PolicyArn = "arn:aws:iam::aws:policy/service-role/AWSLambdaVPCAccessExecutionRole"
            }, customResourceOptions);

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
                    { "ConnectionString", databaseComponent.ConnectionString },
                }
            },
        }, customResourceOptions);

        MigrationInvocation = new Invocation("migration", new()
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
            Provider = customResourceOptions.Provider,
        });
    }
}