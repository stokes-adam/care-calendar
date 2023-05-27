param (
    [Parameter(Mandatory=$true)]
    [string]$user,

    [Parameter(Mandatory=$true)]
    [string]$password,

    [Parameter(Mandatory=$true)]
    [string]$database
)

$ErrorActionPreference = "Stop"

$tag="care-calendar-postgres"

docker rm -f $tag 2>&1 | Out-Null

docker run --name $tag `
    -e POSTGRES_PASSWORD=$password `
    -e POSTGRES_USER=$user `
    -e POSTGRES_DB=$database `
    -p 5432:5432 `
    -d postgres:latest

$connectionString = "Server=localhost;Port=5432;Database=$database;User Id=$user;Password=$password;"

dotnet run $connectionString
