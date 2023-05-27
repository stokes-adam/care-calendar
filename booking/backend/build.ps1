#  get from environment variables
$GITHUB_TOKEN=$env:GITHUB_TOKEN
$GITHUB_RUN_NUMBER=$env:GITHUB_RUN_NUMBER

# stop if not set
$ErrorActionPreference = "Stop"
if($GITHUB_TOKEN -eq $null -or $GITHUB_TOKEN -eq "") {
    Write-Error "GITHUB_TOKEN environment variable is not set"
}
if($GITHUB_RUN_NUMBER -eq $null -or $GITHUB_RUN_NUMBER -eq "") {
    Write-Error "GITHUB_RUN_NUMBER environment variable is not set"
}

$tag="backend-infra"

docker build `
  --build-arg GITHUB_TOKEN=$GITHUB_TOKEN `
  --build-arg GITHUB_RUN_NUMBER=$GITHUB_RUN_NUMBER `
  -f build.dockerfile `
  --tag $tag.

docker run --rm --name $tag `
 -v /var/run/docker.sock:/var/run/docker.sock `
 -v ${PWD}/artifacts:/repo/artifacts `
 --network host `
 $tag `
 dotnet run -p build/build.csproj -c Release -- $args

