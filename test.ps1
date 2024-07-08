# tests.ps1
param(
    [string]$Configuration = "Release",
    [string]$TestType = "All" # Possible values: All, NoCosmosDB, OnlyCosmosDB
)

# Get all test project files
$testProjects = Get-ChildItem -Path "tests" -Recurse -Filter "*.csproj"

# Filter test projects based on the $TestType parameter
if ($TestType -eq "NoCosmosDB") {
    $testProjects = $testProjects | Where-Object { $_.FullName -notmatch "CosmosDB" }
    Write-Host "TestType: $($TestType) => Skipping CosmosDB tests..." -ForegroundColor "Cyan"
} elseif ($TestType -eq "OnlyCosmosDB") {
    $testProjects = $testProjects | Where-Object { $_.FullName -match "CosmosDB" }
    Write-Host "TestType: $($TestType) => Testing only CosmosDB..." -ForegroundColor "Cyan"
}

foreach ($project in $testProjects) {
    Write-Host "Running tests for project: $($project.FullName)"
    dotnet test $project.FullName --configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed for project: $($project.FullName)" -ForegroundColor "Red"
        exit $LASTEXITCODE
    }
}

Write-Host "All tests passed." -ForegroundColor "Green"