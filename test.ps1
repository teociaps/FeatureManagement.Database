# test.ps1
param(
    [string]$Configuration = "Release",
    [string]$TestType = "All" # Possible values: All, Abstractions, CosmosDB, Dapper, EntityFrameworkCore, MongoDB, NHibernate
)

# Get all test project files
$testProjects = Get-ChildItem -Path "tests" -Recurse -Filter "*.csproj"

# Filter test projects based on the $TestType parameter, except for "All"
if ($TestType -ne "All") {
    $testProjects = $testProjects | Where-Object { $_.FullName -match $TestType }
    Write-Host "TestType: $($TestType) => Testing only projects matching '$TestType'..." -ForegroundColor "Cyan"
}

Write-Host "Projects to be tested:" -ForegroundColor "Cyan"
$testProjects | ForEach-Object { Write-Host $_.FullName -ForegroundColor "Cyan" }

foreach ($project in $testProjects) {
    Write-Host "Running tests for project: $($project.FullName)"
    dotnet test $project.FullName --configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed for project: $($project.FullName)"
        exit $LASTEXITCODE
    }
}

Write-Host "All tests passed." -ForegroundColor "Green"