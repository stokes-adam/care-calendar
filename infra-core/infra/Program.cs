using System.Collections.Generic;
using System.Linq;
using components;
using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Acm;
using Pulumi.Aws.CloudFront;
using Pulumi.Aws.CloudFront.Inputs;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.LB;
using Pulumi.Aws.LB.Inputs;
using Pulumi.Aws.Route53;
using Pulumi.Aws.Route53.Inputs;
using Pulumi.Aws.S3;
using DistributionDefaultCacheBehaviorArgs = Pulumi.Aws.CloudFront.Inputs.DistributionDefaultCacheBehaviorArgs;
using DistributionOriginArgs = Pulumi.Aws.CloudFront.Inputs.DistributionOriginArgs;

return await Deployment.RunAsync(() =>
{
    const string domain = "carecalendar.xyz";
    const string wildcardSubdomain = "*." + domain;
    const string staticSubdomain = "static." + domain;
    const string bookingSubdomain = "booking." + domain;
    const string apiSubdomain = "api." + domain;

    // the provider and options
    var provider = new Provider("care-calendar-provider", new ProviderArgs
    {
        Region = "us-east-1",
    });

    var customResourceOptions = new CustomResourceOptions
    {
        Provider = provider
    };

    var zone = new Zone("care-calendar-zone", new ZoneArgs
    {
        Name = domain,
    });

    var certificate = new Certificate("care-calendar-certificate", new CertificateArgs
    {
        DomainName = domain,
        SubjectAlternativeNames = wildcardSubdomain,
        ValidationMethod = "DNS",
    }, customResourceOptions);

    var certValidation = new Record("certValidation", new RecordArgs
    {
        Name = certificate.DomainValidationOptions.Apply(options => options[0].ResourceRecordName!),
        Records = certificate.DomainValidationOptions.Apply(options => options[0].ResourceRecordValue!),
        Ttl = 60,
        Type = certificate.DomainValidationOptions.Apply(options => options[0].ResourceRecordType!),
        ZoneId = zone.Id
    }, customResourceOptions);
    
    var distribution = new DistributionComponent(customResourceOptions, certificate, zone, staticSubdomain);

    var network = new NetworkComponent(customResourceOptions);
    
    var lb = new LoadBalancerComponent(customResourceOptions, network, distribution, zone, apiSubdomain);

    return new Dictionary<string, object?>
    {
        ["bucketId"] = distribution.Bucket.Id,
        ["providerRegion"] = provider.Region,
        ["subnetIds"] = network.SubnetIds,
        ["securityGroupId"] = network.SecurityGroup.Id,
        ["targetGroupArn"] = lb.TargetGroup.Arn
    };
});