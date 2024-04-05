<#
.SYNOPSIS
    Builds the SearchEngines plugin
.DESCRIPTION
    This script builds and packages the SearchEngines plugin. It also
    copies the build to the PowerToys Run Plugins directory.
    i.e. `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins`
.EXAMPLE
    .\Build.ps1
    Builds the SearchEngines plugin using the defaults
.EXAMPLE
    .\Build.ps1 -Platform x64
    Builds the SearchEngines plugin for the x64 platform
#>
[CmdletBinding()]
param(
    # The platform to build the project for. Must be one of: x64, ARM64
    [ValidateSet("x64", "ARM64")]
    [string] $Platform = "x64",

    # The configuration to build the project for. Must be one of: Debug, Release
    [ValidateSet("Debug", "Release")]
    [string] $Configuration = "Release",

    # The target framework to build the project for.
    [string] $TargetFramework = "net8.0-windows",

    # The output directory for the build artifacts
    [string] $OutDir = "$PSScriptRoot\Dist",

    # The path to the PowerToys executable
    [ValidateScript({ Test-Path -Path $_ -PathType Leaf })]
    [string] $PowerToysPath = "C:\Program Files\PowerToys\PowerToys.exe"
)

# Constants
$ProjectName = "SearchEngines" # Project Name
$ProjectFullName = "Community.PowerToys.Run.Plugin.$ProjectName" # Project FullName
$ProjectBinFolder = "$PSScriptRoot\$ProjectFullName\bin" # Project Bin Folder
$PowerToysRunPluginsDirectory = "$env:LOCALAPPDATA\Microsoft\PowerToys\PowerToys Run\Plugins" # PowerToys Run Plugins Directory
$Version = "1.0.0" # Plugin Version Number

# Stop running PowerToys process
Write-Host "Stopping PowerToys process..." -NoNewline
Stop-Process -Name "PowerToys" -Force -ErrorAction SilentlyContinue
Write-Host "✅"

# Clean the bin folder
Write-Host "Cleaning the bin folder..." -NoNewline
if (Test-Path -Path $ProjectBinFolder) {
    Remove-Item -Path "$ProjectBinFolder\*" -Recurse -Force
}
Write-Host "✅"

# Build the project
Write-Host "`nBuilding the project..."
dotnet build "$PSScriptRoot\$ProjectFullName.sln" -c $Configuration /p:Platform=$Platform
Write-Host " "

# Prepare the build for packaging
Write-Host "Preparing the build for packaging..." -NoNewline
Remove-Item -Path "$ProjectBinFolder\*" -Recurse -Include *.xml, *.pdb, PowerToys.*, Wox.*
Write-Host "✅"

# Copy the build to the PowerToys Run Plugins directory
Write-Host "Copying the build to the PowerToys Run Plugins directory..." -NoNewline
Remove-Item -Path "$PowerToysRunPluginsDirectory\$ProjectName" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item -Path "$ProjectBinFolder\$Platform\$Configuration\$TargetFramework" -Destination "$PowerToysRunPluginsDirectory\$ProjectName" -Recurse -Force
Write-Host "✅"

# Package the project
Write-Host "Packaging the project..." -NoNewline
Compress-Archive -Path "$ProjectBinFolder\$Platform\$Configuration\$TargetFramework" -DestinationPath "$OutDir\$ProjectName-v$Version-$Platform.zip" -Force
Write-Host "✅"

# Restart PowerToys
Write-Host "Restarting PowerToys..." -NoNewline
Start-Process -FilePath $PowerToysPath
Write-Host "✅"

Write-Host "`n📦 Build Completed Successfully ✅`n"
