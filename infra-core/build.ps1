# We pass in the environment variables from the runner to the container
$GITHUB_TOKEN=$env:GITHUB_TOKEN
$GITHUB_RUN_NUMBER=$env:GITHUB_RUN_NUMBER

# Stop if I forgot to set the environment variables
$ErrorActionPreference = "Stop"

if ($GITHUB_TOKEN -eq $null -or $GITHUB_TOKEN -eq "") {
  Write-Error "GITHUB_TOKEN environment variable is not set"
}

if ($GITHUB_RUN_NUMBER -eq $null -or $GITHUB_RUN_NUMBER -eq "") {
  Write-Error "GITHUB_RUN_NUMBER environment variable is not set"
}

# Build the container
$tag="infra-core-build"

docker build `
  --build-arg GITHUB_TOKEN=$GITHUB_TOKEN `
  --build-arg GITHUB_RUN_NUMBER=$GITHUB_RUN_NUMBER `
  -f build.dockerfile `
  --tag $tag.

# Run the container
# we give the docker container access to the docker socket so it can build images
# with the arguments passed through to the build.exe
docker run --rm --name $tag `
  -v /var/run/docker.sock:/var/run/docker.sock `
  --network host `
  $tag `
  dotnet run -p build/build.csproj -c Release -- $args
  