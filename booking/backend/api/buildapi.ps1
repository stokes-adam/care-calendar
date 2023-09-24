param (
    [Parameter(Mandatory=$true)]
    [string]$user,

    [Parameter(Mandatory=$true)]
    [string]$password,

    [Parameter(Mandatory=$true)]
    [string]$database
)

$connectionString = "Server=localhost;Port=5432;Database=$database;User Id=$user;Password=$password;"
$tag="care-calendar-api"

docker rm -f $tag 2>&1 | Out-Null

docker build `
  -t $tag `
  -f buildapi.dockerfile `
  .

docker run --rm --name $tag `
  -p 5057:5057 `
  -e ConnectionString=$connectionString `
  -e EncryptionKey=$encryptionKey `
  $tag
