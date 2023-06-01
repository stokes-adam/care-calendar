param (
    [Parameter(Mandatory=$true)]
    [string]$user,

    [Parameter(Mandatory=$true)]
    [string]$password,

    [Parameter(Mandatory=$true)]
    [string]$database,

    [Parameter(Mandatory=$true)]
    [string]$encryptionKey
)

$connectionString = "Server=localhost;Port=5432;Database=$database;User Id=$user;Password=$password;"

dotnet build
dotnet run --ConnectionString $connectionString --EncryptionKey $encryptionKey
