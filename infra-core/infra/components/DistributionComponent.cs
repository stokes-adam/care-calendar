using Pulumi;
using Pulumi.Aws.Acm;
using Pulumi.Aws.CloudFront;
using Pulumi.Aws.CloudFront.Inputs;
using Pulumi.Aws.Route53;
using Pulumi.Aws.Route53.Inputs;
using Pulumi.Aws.S3;

namespace components;

public class DistributionComponent
{
    public DistributionComponent(
        CustomResourceOptions customResourceOptions,
        Certificate certificate,
        Zone zone,
        string staticSubdomain)
    {
        Bucket = new Bucket("care-calendar-bucket", new BucketArgs
        {
            Acl = "private"
        }, customResourceOptions);

        var originAccessControl = new OriginAccessControl("care-calendar-oac", new OriginAccessControlArgs
        {
            OriginAccessControlOriginType = "s3",
            SigningBehavior = "always",
            SigningProtocol = "sigv4"
        }, customResourceOptions);

        Distribution = new Distribution("care-calendar-static-distribution", new DistributionArgs
        {
            Enabled = true,
            Aliases = { staticSubdomain },
            Origins = new DistributionOriginArgs
            {
                DomainName = Bucket.BucketRegionalDomainName,
                OriginId = Bucket.Id,
                OriginAccessControlId = originAccessControl.Id
            },
            DefaultCacheBehavior = new DistributionDefaultCacheBehaviorArgs
            {
                AllowedMethods = { "GET", "HEAD", "OPTIONS" },
                CachedMethods = { "GET", "HEAD", "OPTIONS" },
                TargetOriginId = Bucket.Id,
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
                         ""Resource"": ""{Bucket.Arn}/*"",
                         ""Condition"": {{
                            ""StringEquals"": {{
                                ""AWS:SourceArn"": ""{Distribution.Arn}""
                            }}
                        }}
                    }}
                ]
            }}");

        var bucketPolicy = new BucketPolicy("static-bucket-policy", new BucketPolicyArgs
        {
            Bucket = Bucket.BucketName,
            Policy = policy
        }, customResourceOptions);

        var staticSubdomainARecord = new Record("care-calendar-arecord", new RecordArgs
        {
            Name = staticSubdomain,
            Type = "A",
            ZoneId = zone.Id,
            Aliases = new RecordAliasArgs
            {
                Name = Distribution.DomainName,
                ZoneId = Distribution.HostedZoneId,
                EvaluateTargetHealth = false,
            },
        }, customResourceOptions);
    }
    
    public Bucket Bucket { get; }
    public Distribution Distribution { get; }
}