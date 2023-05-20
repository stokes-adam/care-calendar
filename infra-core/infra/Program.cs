using System.Collections.Generic;
using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Acm;
using Pulumi.Aws.CloudFront;
using Pulumi.Aws.CloudFront.Inputs;
using Pulumi.Aws.LightSail.Inputs;
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
    
    var mainDomainARecord = new Record("care-calendar-arecord", new RecordArgs
    {
        Name = domain,
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
        ["providerRegion"] = provider.Region
    };
});