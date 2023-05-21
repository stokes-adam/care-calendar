using Ductus.FluentDocker;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string
    Clean = "clean",
    Publish = "publish",
    Build = "build-container",
    Push = "push-container",
    ImageName = "frontend-infra";

var tempPath = Path.Combine(Environment.CurrentDirectory, "temp");

Target(Clean, () =>
{
    Utils.CleanDirectory(tempPath);
});

Target(Publish, () =>
{
    // build a single file executable for linux
    Run("dotnet", $"publish infra/infra.csproj -r linux-x64 -c Release -p:PublishSingleFile=true -o temp/app");
});

Target(Build, () =>
{
    Fd.DefineImage(ImageName)
        .From("pulumi/pulumi-dotnet:latest")
        .Environment("DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1")
        .Copy("/app", "/app")
        .WorkingFolder("temp")
        .UseWorkDir("/app")
        .Build();

    Run("docker", $"tag {ImageName}:latest ghcr.io/stokes-adam/care-calendar/{ImageName}:latest");
}); 

Target(Push, () =>
{
    Run("docker", $"push ghcr.io/stokes-adam/care-calendar/{ImageName}:latest");
});

Target("default", DependsOn(Clean, Publish, Build));

await RunTargetsAndExitAsync(args); 