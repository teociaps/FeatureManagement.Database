# build.ps1
param(
    [string]$Configuration = "Release"
)

$green = "`e[32m"
$red = "`e[31m"
$reset = "`e[0m"

# Restore dependencies
Write-Host "Restoring dependencies..."
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "${red}dotnet restore failed${reset}"
    exit $LASTEXITCODE
}

# Build the project
Write-Host "Building project..."
dotnet build --no-restore --configuration $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "${red}dotnet build failed${reset}"
    exit $LASTEXITCODE
}

Write-Host "${green}Build completed successfully.${reset}"