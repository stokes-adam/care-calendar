using Ductus.FluentDocker;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string clean = "clean";
const string publish = "publish";
const string buildContainer = "build-container";
const string pushContainer = "push-container";
const string containerName = "backend-infra";

var imageTag = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER") ?? "GITHUB_RUN_NUMBER-NOT-SET";
var tempPath = Path.Combine(Environment.CurrentDirectory, "temp");

Target(clean, () =>
{
    Utils.CleanDirectory(tempPath);
});

Target(publish, DependsOn(clean), () =>
{
    Run("dotnet", $"publish infra/infra.csproj -r linux-x64 -c Release -p:PublishSingleFile=true -o {tempPath}/app");
});

Target(buildContainer, () =>
{
    Fd.DefineImage(containerName)
        .From("pulumi/pulumi-dotnet:latest")
        .Environment("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1")
        .Copy("/app", "/app")
        .WorkingFolder(tempPath)
        .UseWorkDir("/app")
        .Build();
    
    Run("docker", $"tag {containerName}:latest ghcr.io/stokes-adam/care-calendar/{containerName}:{imageTag}");
});

Target(pushContainer, () =>
{
    Run("docker", $"push ghcr.io/stokes-adam/care-calendar/{containerName}:{imageTag}");
});

Target("default", DependsOn(clean, publish, buildContainer));

await RunTargetsAndExitAsync(args);