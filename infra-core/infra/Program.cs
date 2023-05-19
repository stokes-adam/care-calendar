using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.S3;

return await Deployment.RunAsync(() =>
{
    const string domain = "carecalendar.xyz";
    const string bookingSubdomain = "booking.carecalendar.xyz"; // this is where the booking app will be hosted
    const string apiSubdomain = "api.carecalendar.xyz"; // this is where the api will be hosted

    var provider = new Provider("care-calendar-provider", new ProviderArgs
    {
        Region = "us-east-1",
    });
    
    var options = new CustomResourceOptions
    {
        Provider = provider
    };
    
    var bucket = new Bucket("care-calendar-bucket", new BucketArgs
    {
    }, options);
    
    // hosted zone
    // s3 bucket
    // cloudfront distribution
    // certificate
    // route53 record

    // rds instance
    // ecs

});