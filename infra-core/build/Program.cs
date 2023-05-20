using Ductus.FluentDocker;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string Clean = "clean";
const string Publish = "publish";
const string BuildContainer = "build-container";
const string PushContainer = "push-container";
const string ContainerName = "infra-core";

var imageTag = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER") ?? "GITHUB_RUN_NUMBER-NOT-SET";
var tempPath = Path.Combine(Environment.CurrentDirectory, "temp");

Target(Clean, () =>
{
 Utils.CleanDirectory(tempPath);
});

Target(Publish, DependsOn(Clean), () =>
{
 Run("dotnet", $"publish infra/infra-core.csproj -r linux-x64 -c Release -p:PublishSingleFile=true -o {tempPath}/app");
});

Target(BuildContainer, () =>
{
 Fd.DefineImage(ContainerName)
  .From("pulumi/pulumi-dotnet:latest")
  .Environment("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1")
  .Copy("/app", "/app")
  .WorkingFolder(tempPath)
  .UseWorkDir("/app")
  .Build();
    
 Run("docker", $"tag {ContainerName}:latest ghcr.io/care-calendar/{ContainerName}:{imageTag}");
});

Target(PushContainer, () =>
{
 Run("docker", $"push ghcr.io/care-calendar/{ContainerName}:{imageTag}");
});

Target("default", DependsOn(Clean, Publish, BuildContainer));

await RunTargetsAndExitAsync(args);
