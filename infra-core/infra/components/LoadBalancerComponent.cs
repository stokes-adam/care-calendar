using Pulumi;
using Pulumi.Aws.LB;
using Pulumi.Aws.LB.Inputs;
using Pulumi.Aws.Route53;
using Pulumi.Aws.Route53.Inputs;

namespace components;

public class LoadBalancerComponent
{
    public LoadBalancerComponent(
        CustomResourceOptions customResourceOptions,
        NetworkComponent network,
        DistributionComponent distribution,
        Zone zone,
        string apiSubdomain)
    {
        var alb = new LoadBalancer("care-calendar-alb", new LoadBalancerArgs
        {
            Internal = false,
            SecurityGroups = { network.SecurityGroup.Id },
            Subnets = network.SubnetIds,
        }, customResourceOptions);

        TargetGroup = new TargetGroup("tg", new TargetGroupArgs
        {
            Port = 5057,
            Protocol = "HTTP",
            HealthCheck = new TargetGroupHealthCheckArgs
            {
                Path = "/", // Change this to the health check endpoint of your service if you have one
                Interval = 30
            },
            VpcId = network.Vpc.Id
        }, customResourceOptions);

        var listener = new Listener("listener", new ListenerArgs
        {
            LoadBalancerArn = alb.Arn,
            Port = 80, // HTTP, use 443 for HTTPS
            DefaultActions =
            {
                new ListenerDefaultActionArgs
                {
                    Type = "forward",
                    TargetGroupArn = TargetGroup.Arn
                }
            }
        }, customResourceOptions);

        var apiSubdomainARecord = new Record("care-calendar-api-arecord", new RecordArgs
        {
            Name = apiSubdomain,
            Type = "A",
            ZoneId = zone.Id,
            Aliases = new RecordAliasArgs
            {
                Name = distribution.Distribution.DomainName,
                ZoneId = distribution.Distribution.HostedZoneId,
                EvaluateTargetHealth = false,
            },
        }, customResourceOptions);
    }
    
    public TargetGroup TargetGroup { get; }
}