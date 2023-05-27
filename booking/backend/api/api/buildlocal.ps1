$ErrorActionPreference = "Stop"

$tag="care-calendar-booking-api"

docker rm -f $tag 2>&1 | Out-Null

docker rmi $tag 2>&1 | Out-Null

docker build `
  -f build.dockerfile `
  --tag $tag.

docker run --rm --name $tag `
  -v ${PWD}/artifacts:/repo/artifacts `
  -p 5057:5057 `
  $tag `
  dotnet watch run --no-launch-profile
