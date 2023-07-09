using Ductus.FluentDocker;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string clean = "clean";
const string publish = "publish";
const string buildContainer = "build-container";
const string pushContainer = "push-container";
const string containerName = "backend-infra";
const string apiContainerName = "backend-api";
const string dbUpdateLambdaContainerName = "db-update-lambda";

var imageTag = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER") ?? "GITHUB_RUN_NUMBER-NOT-SET";
var tempPath = Path.Combine(Environment.CurrentDirectory, "temp");

Target(clean, () =>
{
    Utils.CleanDirectory(tempPath);
});

Target(publish, DependsOn(clean), () =>
{
    // infra
    Run("dotnet", $"publish infra/infra.csproj -r linux-x64 -c Release -p:PublishSingleFile=true -o {tempPath}/app");
    
    // api
    Run("dotnet", $"publish api/api/api.csproj -r linux-x64 --self-contained true -c Release -p:PublishSingleFile=true -o {tempPath}/api");
    
    // db-update-lambda
    Run("dotnet", $"publish db/update/update.csproj -r linux-x64 -c Release -p:PublishSingleFile=true -o {tempPath}/db-update-lambda");
});

Target(buildContainer, () =>
{
    Fd.DefineImage(containerName)
        .From("pulumi/pulumi-dotnet:latest")
        .Environment("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1")
        .Environment($"GITHUB_RUN_NUMBER={imageTag}")
        .Copy("/app", "/app")
        .WorkingFolder(tempPath)
        .UseWorkDir("/app")
        .Build();
    
    Run("docker", $"tag {containerName}:latest ghcr.io/stokes-adam/care-calendar/{containerName}:{imageTag}");
    
    // Build the API container
    Fd.DefineImage(apiContainerName)
        .From("mcr.microsoft.com/dotnet/aspnet:6.0")
        .Environment("ASPNETCORE_URLS=http://+:5057")
        .Copy("/api", "/api")
        .WorkingFolder(tempPath)
        .UseWorkDir("/api")
        .Entrypoint("./api")
        .Build();
    
    Run("docker", $"tag {apiContainerName}:latest ghcr.io/stokes-adam/care-calendar/{apiContainerName}:{imageTag}");
    
    // Build the db-update-lambda container
    Fd.DefineImage(dbUpdateLambdaContainerName)
        .From("mcr.microsoft.com/dotnet/runtime:6.0")
        .Copy("/db-update-lambda", "/db-update-lambda")
        .WorkingFolder(tempPath)
        .UseWorkDir("/db-update-lambda")
        .Entrypoint("./db-update-lambda")
        .Build();
    
    Run("docker", $"tag {dbUpdateLambdaContainerName}:latest ghcr.io/stokes-adam/care-calendar/{dbUpdateLambdaContainerName}:{imageTag}");
});

Target(pushContainer, () =>
{
    Run("docker", $"push ghcr.io/stokes-adam/care-calendar/{containerName}:{imageTag}");
    Run("docker", $"push ghcr.io/stokes-adam/care-calendar/{apiContainerName}:{imageTag}");
    Run("docker", $"push ghcr.io/stokes-adam/care-calendar/{dbUpdateLambdaContainerName}:{imageTag}");
});

Target("default", DependsOn(clean, publish, buildContainer));

await RunTargetsAndExitAsync(args);