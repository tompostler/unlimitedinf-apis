# Tom Postler, 2017-03-22
# See if package is already published

# Set CWD to script location
Push-Location $PSScriptRoot
[Environment]::CurrentDirectory = $PWD

# Get the version from the xml
[xml]$packageXml = Get-Content ".\Unlimitedinf.Apis.xml";
$version = $packageXml.Project.PropertyGroup.Version
Write-Host "Found version '$version' in Unlimitedinf.Apis.xml";

# Get the nuget package repository
$pkgNuGet_Core = "NuGet.Core." + (([xml](Get-Content ".\packages.config")).packages.package | where {$_.id -eq "NuGet.Core"}).version
[Reflection.Assembly]::LoadFile((Resolve-Path "..\packages\$pkgNuGet_Core\lib\net40-Client\NuGet.Core.dll").Path) | Out-Null;
$repo = [NuGet.PackageRepositoryFactory]::Default.CreateRepository("https://packages.nuget.org/api/v2");

# Check if current version is already published
if (($repo.FindPackagesById("Unlimitedinf.Apis") | ? {$_.Version -eq $version} | Measure-Object).Count -eq 1) {
    $msg = "Nuget package Unlimitedinf.Apis.$version already published.";
    Write-Warning $msg;
    Write-Error $msg;
} else {
    Write-Host "Nuget package Unlimitedinf.Apis.$version not found on nuget.org";
}

# Restore CWD
Pop-Location
[Environment]::CurrentDirectory = $PWD
