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
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    # Path to the PowerToys installation
    [Alias("Source", "Src")]
    [string] $Path = "C:\Program Files\PowerToys",

    # Destination path to copy the files to
    [Alias("Target")]
    [string] $Dest = $PSScriptRoot
)

# The list of items to copy
$Items = @(
    "PowerToys.Common.UI.dll",
    "PowerToys.ManagedCommon.dll",
    "PowerToys.Settings.UI.Lib.dll",
    "Wox.Infrastructure.dll",
    "Wox.Plugin.dll"
)

# Copy the items to the destination
foreach ($Item in $Items) {
    $From = Join-Path -Path $Path -ChildPath $Item
    $To = Join-Path -Path $Dest -ChildPath $Item
    Copy-Item -Path $From -Destination $To -Force
}
