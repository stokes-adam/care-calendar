using System.Collections.Generic;
using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Acm;
using Pulumi.Aws.CloudFront;
using Pulumi.Aws.CloudFront.Inputs;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;
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
    
    var bucket = new Bucket("care-calendar-bucket", new BucketArgs
    {
        Acl = "private"
    }, customResourceOptions);
    
    var originAccessControl = new OriginAccessControl("care-calendar-oac", new OriginAccessControlArgs
    {
        OriginAccessControlOriginType = "s3",
        SigningBehavior = "always",
        SigningProtocol = "sigv4"
    }, customResourceOptions);

    var distribution = new Distribution("care-calendar-static-distribution", new DistributionArgs
    {
        Enabled = true,
        Aliases = { staticSubdomain },
        DefaultRootObject = "index.html",
        Origins = new DistributionOriginArgs
        {
            DomainName = bucket.BucketRegionalDomainName,
            OriginId = bucket.Id,
            OriginAccessControlId = originAccessControl.Id
        },
        DefaultCacheBehavior = new DistributionDefaultCacheBehaviorArgs
        {
            AllowedMethods = { "GET", "HEAD", "OPTIONS" },
            CachedMethods = { "GET", "HEAD", "OPTIONS" },
            TargetOriginId = bucket.Id,
            ForwardedValues = new DistributionDefaultCacheBehaviorForwardedValuesArgs
            {
                Cookies = new DistributionDefaultCacheBehaviorForwardedValuesCookiesArgs
                {
                    Forward = "none"
                },
                QueryString = false
            },
            ViewerProtocolPolicy = "redirect-to-https"
        },
        ViewerCertificate = new DistributionViewerCertificateArgs
        {
            AcmCertificateArn = certificate.Arn,
            SslSupportMethod = "sni-only"
        },
        Restrictions = new DistributionRestrictionsArgs
        {
            GeoRestriction = new DistributionRestrictionsGeoRestrictionArgs
            {
                RestrictionType = "none"
            }
        }
    });
    
    var policy = Output.Format($@"{{
        ""Version"": ""2012-10-17"",
        ""Statement"": [
            {{
                 ""Effect"": ""Allow"",
                 ""Principal"": {{
                    ""Service"": ""cloudfront.amazonaws.com""
                 }},
                 ""Action"": ""s3:GetObject"",
                 ""Resource"": ""{bucket.Arn}/*"",
                 ""Condition"": {{
                    ""StringEquals"": {{
                        ""AWS:SourceArn"": ""{distribution.Arn}""
                    }}
                }}
            }}
        ]
    }}");

    var bucketPolicy = new BucketPolicy("static-bucket-policy", new BucketPolicyArgs{
        Bucket = bucket.BucketName,
        Policy = policy
    }, customResourceOptions);
    
    var staticSubdomainARecord = new Record("care-calendar-arecord", new RecordArgs
    {
        Name = staticSubdomain,
        Type = "A",
        ZoneId = zone.Id,
        Aliases = new RecordAliasArgs
        {
            Name = distribution.DomainName,
            ZoneId = distribution.HostedZoneId,
            EvaluateTargetHealth = false,
        },
    }, customResourceOptions);





    var vpc = new Vpc("vpc", new VpcArgs
    {
        CidrBlock = "10.0.0.0/16",
        EnableDnsHostnames = true,
        EnableDnsSupport = true,
    }, customResourceOptions);

    var internetGateway = new InternetGateway("internetGateway", new(), customResourceOptions);

    var vpcInternetGatewayAttachment = new InternetGatewayAttachment("vpcInternetGatewayAttachment", new()
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
        CidrBlock = "10.0.3.0/24",
        AvailabilityZone = "us-east-1a"
    }, customResourceOptions);

    var subnet2 = new Subnet("subnet2", new SubnetArgs
    {
        VpcId = vpc.Id,
        CidrBlock = "10.0.4.0/24",
        AvailabilityZone = "us-east-1b"
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
                CidrBlocks = { "0.0.0.0/0" },
            },
            new SecurityGroupIngressArgs
            {
                Protocol = "tcp",
                FromPort = 5057,
                ToPort = 5057,
                CidrBlocks = { "0.0.0.0/0" },
            }
        },
        Egress =
        {
            new SecurityGroupEgressArgs
            {
                Protocol = "-1",
                FromPort = 0,
                ToPort = 0,
                CidrBlocks = { "0.0.0.0/0" }
            }
        }
    }, customResourceOptions);
    
    
    var alb = new LoadBalancer("care-calendar-alb", new LoadBalancerArgs
    {
        Internal = false,
        SecurityGroups = { securityGroup.Id },
        Subnets = { subnet1.Id, subnet2.Id },
    }, customResourceOptions);
    
    var targetGroup = new TargetGroup("tg", new TargetGroupArgs
    {
        Port = 5057,
        Protocol = "HTTP",
        HealthCheck = new TargetGroupHealthCheckArgs
        {
            Path = "/", // Change this to the health check endpoint of your service if you have one
            Interval = 30
        },
        VpcId = vpc.Id
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
                TargetGroupArn = targetGroup.Arn
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
            Name = distribution.DomainName,
            ZoneId = distribution.HostedZoneId,
            EvaluateTargetHealth = false,
        },
    }, customResourceOptions);

    return new Dictionary<string, object?>
    {
        ["bucketId"] = bucket.Id,
        ["providerRegion"] = provider.Region,
        ["subnet1Id"] = subnet1.Id,
        ["subnet2Id"] = subnet2.Id,
        ["securityGroupId"] = securityGroup.Id,
        ["targetGroupArn"] = targetGroup.Arn
    };
});