# build.ps1
param(
    [string]$Configuration = "Release"
)

# Restore dependencies
Write-Host "Restoring dependencies..."
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet restore failed"
    exit $LASTEXITCODE
}

# Build the project
Write-Host "Building project..."
dotnet build --no-restore --configuration $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet build failed"
    exit $LASTEXITCODE
}

Write-Host "Build completed successfully." -ForegroundColor "Green"