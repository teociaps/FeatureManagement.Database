# build.ps1
param(
    [string]$Configuration = "Release",
    [string]$SolutionFile = "FeatureManagement.Database.sln"
)

# Restore dependencies
Write-Host "Restoring dependencies..."
dotnet restore $SolutionFile
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet restore failed"
    exit $LASTEXITCODE
}

# Build the project
Write-Host "Building project..."
dotnet build $SolutionFile --no-restore --configuration $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet build failed"
    exit $LASTEXITCODE
}

Write-Host "Build completed successfully." -ForegroundColor "Green"