# run-tests.ps1
param(
    [string]$Configuration = "Release"
)

# Get all test project files
$testProjects = Get-ChildItem -Path "tests" -Recurse -Filter "*.csproj"

foreach ($project in $testProjects) {
    Write-Host "Running tests for project: $($project.FullName)"
    dotnet test $project.FullName --configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed for project: $($project.FullName)"
        exit $LASTEXITCODE
    }
}

Write-Host "All tests passed."