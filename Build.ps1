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
Stop-Process -Name "PowerToys" -Force -ErrorAction SilentlyContinue

# Clean the bin folder
if (Test-Path -Path $ProjectBinFolder) {
    Remove-Item -Path "$ProjectBinFolder\*" -Recurse -Force
}

# Build the project
dotnet build "$PSScriptRoot\$ProjectFullName.sln" -c $Configuration /p:Platform=$Platform

# Prepare the build for packaging
Remove-Item -Path "$ProjectBinFolder\*" -Recurse -Include *.xml, *.pdb, PowerToys.*, Wox.*

# Copy the build to the PowerToys Run Plugins directory
Remove-Item -Path "$PowerToysRunPluginsDirectory\$ProjectName" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item -Path "$ProjectBinFolder\$Platform\Release\net8.0-windows" -Destination "$PowerToysRunPluginsDirectory\$ProjectName" -Recurse -Force

# Package the project
Compress-Archive -Path "$ProjectBinFolder\$Platform\Release\net8.0-windows" -DestinationPath "$PSScriptRoot\Dist\$ProjectName-$Version-$Platform.zip" -Force

# Restart PowerToys
Start-Process -FilePath $PowerToysPath
