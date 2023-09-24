param (
    [Parameter(Mandatory=$true)]
    [string]$USER,

    [Parameter(Mandatory=$true)]
    [string]$PASSWORD,

    [Parameter(Mandatory=$true)]
    [string]$DATABASE
)

$tag="care-calendar-migration"

$postgresIP = docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' care-calendar-postgres

$CONNECTION_STRING = "Server=$postgresIP;Port=5432;Database=$DATABASE;User Id=$USER;Password=$PASSWORD;"

docker rm -f $tag 2>&1 | Out-Null

docker build `
    -f migratelocal.dockerfile `
    --tag $tag.

docker run --name $tag `
    -e ConnectionString=$CONNECTION_STRING `
    --network host `
    $tag `
    dotnet update.dll
