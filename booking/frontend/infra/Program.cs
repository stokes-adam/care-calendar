using Pulumi;
using Pulumi.Aws.S3;
using System.Collections.Generic;
using Pulumi.Aws;

return await Deployment.RunAsync(() =>
{
    var coreStack = new StackReference("care-calendar/infra-core/default");
    var bucketId = coreStack.RequireOutput("bucketId").Apply(id => id.ToString());
    var providerRegion = coreStack.RequireOutput("providerRegion").Apply(region => region.ToString());
    
    var provider = new Provider("us-east-1", new ProviderArgs
    {
        Region = providerRegion,
    });
    
    var customResourceOptions = new CustomResourceOptions {
        Provider = provider,
        DependsOn = coreStack
    };
    
    // Upload files to S3 bucket
    const string wwwDistPath = "./wwwroot";
    var files = Directory.GetFiles(wwwDistPath, "*", SearchOption.AllDirectories);
    
    Console.WriteLine($"Uploading {files.Length} files to S3 bucket");
    
    foreach (var file in files)
    {
        Console.WriteLine($"Uploading {file}");
    }
    
    foreach (var file in files)
    {
        var relativePath = Path
            .GetRelativePath(wwwDistPath, file)
            .Replace("\\", "/");

        var s3Path = $"{relativePath}";

        var s3Object = new BucketObject($"care-calendar-{s3Path}", new BucketObjectArgs
        {
            Bucket = bucketId,
            Source = new FileAsset(file),
            Key = s3Path,
            ContentType = Utils.GetContentType(file)
            }, customResourceOptions);
    }
});
