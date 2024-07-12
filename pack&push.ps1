# pack&push.ps1
param (
    [string]$Configuration = "Release",
    [string]$PackageOutput = "$(pwd)/artifacts",
    [string]$NuGetSourceUrl = "https://api.nuget.org/v3/index.json",
    [string]$NuGetApiKey
)

# Ensure the output directory exists
if (-not (Test-Path -Path $PackageOutput)) {
    New-Item -ItemType Directory -Path $PackageOutput | Out-Null
}

# Iterate over each project in the src directory
Get-ChildItem -Path src -Filter FeatureManagement.* -Directory | ForEach-Object {
    $projectPath = $_.FullName
    $projectName = $_.Name
    $csprojPath = Join-Path -Path $projectPath -ChildPath "$projectName.csproj"

    try {
        $version = dotnet msbuild $csprojPath -nologo -t:GetVersion -v:q -p:OutputProperty=Version

        if (-not $version) {
            throw "Failed to retrieve version for project $projectName"
        }

        $packageName = "$projectName.$version.nupkg"

        dotnet pack $projectPath --configuration $Configuration --output $PackageOutput

        $packageExists = dotnet nuget search $projectName --version $version --source $NuGetSourceUrl | Select-String -Pattern $version

        if (-not $packageExists) {
            dotnet nuget push "$PackageOutput/$packageName" -k $NuGetApiKey -s $NuGetSourceUrl
            Write-Host "Package $packageName version $version pushed." -ForegroundColor "Green"
        } else {
            Write-Host "Package $packageName version $version already exists on NuGet. Skipping push." -ForegroundColor "Yellow"
        }
    } catch {
        Write-Error "Error processing project $projectName: $_"
    }
}

Write-Host "Deploy finished."