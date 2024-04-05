<#
.SYNOPSIS
    Builds the SearchEngines plugin
.DESCRIPTION
    This script builds and packages the SearchEngines plugin. It also
    copies the build to the PowerToys Run Plugins directory.
    i.e. `%LOCALAPPDATA%\Microsoft\PowerToys\Run\Plugins`
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
    [string] $Configuration = "Release"
)

$ProjectName = "SearchEngines" # Project Name
$ProjectFullName = "Community.PowerToys.Run.Plugin.$ProjectName" # Project FullName
$Version = "1.0.0" # Plugin Version Number

# Stop running PowerToys process
Stop-Process -Name "PowerToys" -Force -ErrorAction SilentlyContinue

# Clean the bin folder
if (Test-Path -Path "$PSScriptRoot\$ProjectFullName\bin") {
    Remove-Item -Path "$PSScriptRoot\$ProjectFullName\bin\*" -Recurse -Force
}

# Build the project
dotnet build "$PSScriptRoot\$ProjectFullName.sln" -c $Configuration /p:Platform=$Platform

# Package the project
Remove-Item -Path "$PSScriptRoot\$ProjectFullName\bin\*" -Recurse -Include *.xml, *.pdb, PowerToys.*, Wox.*
Rename-Item -Path "$PSScriptRoot\$ProjectFullName\bin\$Platform\Release" -NewName $ProjectName    
Compress-Archive -Path "$PSScriptRoot\$ProjectFullName\bin\$Platform\$ProjectName" -DestinationPath "$PSScriptRoot\Dist\$ProjectName-$Version-$Platform.zip" -Force

