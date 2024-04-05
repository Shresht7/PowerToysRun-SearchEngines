$ProjectName = "SearchEngines" # Project Name
$ProjectFullname = "Community.PowerToys.Run.Plugin.$ProjectName" # Project FullName
$Version = "1.0.0" # Plugin Version Number

# Supported Platforms
$Platforms = @(
    "x64"
    # , "ARM64"
)

# Stop running PowerToys process
Stop-Process -Name "PowerToys" -Force -ErrorAction SilentlyContinue

# Build the project for each platform
foreach ($Platform in $Platforms) {

    # Clean the bin folder
    if (Test-Path -Path "$PSScriptRoot\$ProjectFullName\bin") {
        Remove-Item -Path "$PSScriptRoot\$ProjectFullName\bin\*" -Recurse -Force
    }

    # Build the project
    dotnet build "$PSScriptRoot\$ProjectFullName.sln" -c Release /p:Platform=$Platform

    # Package the project
    Remove-Item -Path "$PSScriptRoot\$ProjectFullName\bin\*" -Recurse -Include *.xml, *.pdb, PowerToys.*, Wox.*
    Rename-Item -Path "$PSScriptRoot\$ProjectFullName\bin\$Platform\Release" -NewName $ProjectName    
    Compress-Archive -Path "$PSScriptRoot\$ProjectFullName\bin\$Platform\$ProjectName" -DestinationPath "$PSScriptRoot\Dist\$ProjectName-$Version-$Platform.zip" -Force

}
