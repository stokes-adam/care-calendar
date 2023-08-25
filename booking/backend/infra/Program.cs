using infra;
using Pulumi;
using Pulumi.Aws;

return await Deployment.RunAsync(() =>
{
    var core = new StackReference("care-calendar/infra-core/default");

    var customResourceOptions = new CustomResourceOptions
    {
        DependsOn = core,
        Provider = new Provider("us-east-1", new ProviderArgs
        {
            Region = core.RequireOutput("providerRegion").Apply(region => region.ToString())!
        }),
    };

    var db = new DatabaseComponent(core, customResourceOptions);
    var svc = new ServiceComponent(core, customResourceOptions);
    var m = new MigrationComponent(core, db, customResourceOptions);
});
