using Ductus.FluentDocker;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string clean = "clean";
const string publish = "publish";
const string build = "build-container";
const string push = "push-container";
const string containerName = "frontend-infra";

var imageTag = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER") ?? "GITHUB_RUN_NUMBER-NOT-SET";
var tempPath = Path.Combine(Environment.CurrentDirectory, "temp");

Target(clean, () =>
{
    Utils.CleanDirectory(tempPath);
});

Target(publish, () =>
{
    // build a single file executable for linux
    Run("dotnet", $"publish infra/infra.csproj -r linux-x64 -c Release -p:PublishSingleFile=true -o temp/app");
});

Target(build, () =>
{
    Utils.CopyDirectory("wwwroot/dist", $"{tempPath}/wwwroot");

    Fd.DefineImage(containerName)
        .From("pulumi/pulumi-dotnet:latest")
        .Environment("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1")
        .Copy("/app", "/app")
        .WorkingFolder("temp")
        .UseWorkDir("/app")
        .Build();

    Run("docker", $"tag {containerName}:latest ghcr.io/stokes-adam/care-calendar/{containerName}:{imageTag}");
}); 

Target(push, () =>
{
    Run("docker", $"push ghcr.io/stokes-adam/care-calendar/{containerName}:{imageTag}");
});

Target("default", DependsOn(clean, publish, build));

await RunTargetsAndExitAsync(args); 
