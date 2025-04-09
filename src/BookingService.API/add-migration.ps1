param (
    [string]$migrationName
)

if (-not $migrationName) {
    Write-Host "Please provide a migration name."
    exit 1
}

dotnet ef migrations add $migrationName --output-dir "Infrastructure\Migrations"