<#
.SYNOPSIS
    Source the project references from the PowerToys installation.
.DESCRIPTION
    This script copies the project references from the PowerToys installation to the current project.
.EXAMPLE
    . .\Get-ProjectReferences.ps1
    Invoke the script to retrieve the project references from the PowerToys installation.
.EXAMPLE
    . .\Get-ProjectReferences.ps1 -WhatIf
    Perform a dry-run.
.EXAMPLE
    . .\Get-ProjectReferences.ps1 -Confirm
    Prompt for confirmation before copying the files.
.EXAMPLE
    . .\Get-ProjectReferences.ps1 -Path "C:\Program Files\PowerToys"
    Specify the path to the PowerToys installation.
.EXAMPLE
    . .\Get-ProjectReferences.ps1 -Dest "C:\Projects\MyProject"
    Specify the destination path to copy the files to.
.EXAMPLE
    . .\Get-ProjectReferences.ps1 -Symlink
    Create symlinks instead of copying the files.
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    # Path to the PowerToys installation
    [Alias("Source", "Src")]
    [string] $Path = "C:\Program Files\PowerToys",

    # Destination path to copy the files to
    [Alias("Target")]
    [string] $Dest = $PSScriptRoot,

    # Whether to create symlinks instead of copying the files
    [switch] $Symlink
)

# The list of library items
$Items = @(
    "PowerToys.Common.UI.dll",
    "PowerToys.ManagedCommon.dll",
    "PowerToys.Settings.UI.Lib.dll",
    "Wox.Infrastructure.dll",
    "Wox.Plugin.dll"
)

foreach ($Item in $Items) {

    # Construct the source and destination paths
    $From = Join-Path -Path $Path -ChildPath $Item
    $To = Join-Path -Path $Dest -ChildPath $Item

    # Remove existing files
    Remove-Item -Path $To -Force -ErrorAction SilentlyContinue

    # If $Symlink switch was provided, create a symbolic links...
    if ($Symlink) {
        New-Item -ItemType SymbolicLink -Path $To -Target $From -Force
    } else {
        # ...otherwise, copy the files to the destination
        Copy-Item -Path $From -Destination $To -Force
    }

}
