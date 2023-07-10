using Pulumi;
using Pulumi.Aws.Iam;
using Pulumi.Aws.Lambda;
using Pulumi.Aws.Lambda.Inputs;

namespace infra;

public class MigrationInfra
{
   public MigrationInfra(DbInfra dbInfra, NetworkInfra networkInfra, CustomResourceOptions customResourceOptions)
   {
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
      
      var migrationRolePolicyAttachment = new RolePolicyAttachment("migrationRolePolicyAttachment", new RolePolicyAttachmentArgs
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
          VpcConfig =  new FunctionVpcConfigArgs
          {
              SubnetIds = { networkInfra.Subnet1Id, networkInfra.Subnet2Id },
              SecurityGroupIds = { networkInfra.SecurityGroupId }
          },
          Environment = new FunctionEnvironmentArgs
          {
              Variables =
              {
                  { "ConnectionString", dbInfra.ConnectionString },
              }
          },
      }, customResourceOptions);
   }
}