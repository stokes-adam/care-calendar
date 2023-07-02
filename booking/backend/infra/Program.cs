using infra;
using Pulumi;
using Pulumi.Aws;

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
    
    var networkInfra = new NetworkInfra(customResourceOptions);
    var dbInfra = new DbInfra(networkInfra, customResourceOptions);
    var serviceInfra = new ServiceInfra(networkInfra, customResourceOptions);
});
