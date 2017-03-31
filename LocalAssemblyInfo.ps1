﻿# Tom Postler, 2017-03-16
# Generate specific assembly information

param(
	[Parameter(Mandatory=$true)][string]$ProjectDir,
	[Parameter(Mandatory=$true)][string]$AssemblyId
)

# Set CWD to script location
Push-Location $PSScriptRoot
[Environment]::CurrentDirectory = $PWD

# Version info vars
$Major, $Minor, $Patch, $Prerelease = @(0,0,0,0);

# If we have a version info for this assembly, then use it instead
$PackageInfoPath = ".\Unlimitedinf.Apis.Package\$AssemblyId.xml";
if (Test-Path $PackageInfoPath) {
	Write-Host "Found $PackageInfoPath";

	# Get the necesary information out of the XML
	[xml]$packageInfo = Get-Content $PackageInfoPath;
	$Version = $packageInfo.Project.PropertyGroup.Version;
	$Prerelease = $packageInfo.Project.PropertyGroup.LastUpdated;

	# Remove anything that would disagree with AssemblyVersion
	if ($Version.Contains("-")) {
		$Version = $Version.Split("-")[0];
	}
	if ($Version.Contains("+")) {
		$Version = $Version.Split("+")[0];
	}

	# Parse out the SemVer
	$Major, $Minor, $Patch = $Version.Split('.');

	# Set the prerelease to the tenths of days since the version string was last updated
	$Prerelease = [Math]::Truncate(([DateTime]::Now - [DateTime]::Parse($Prerelease)).TotalDays * 10);
}

# Guard against efficiency?
if (!(Test-Path "$ProjectDir\Properties")) {
	New-Item "$ProjectDir\Properties" -ItemType Directory > $null;
}

# The file contents
@"
//
// This code was generated by a tool. Any changes made manually will be lost the next time this code is regenerated.
//

using System.Reflection;

[assembly: AssemblyTitle("$AssemblyId")]
[assembly: AssemblyProduct("$AssemblyId")]

[assembly: AssemblyVersion("{0}.{1}.0.0")]
[assembly: AssemblyFileVersion("{0}.{1}.{2}.{3}")]
"@ -f $Major, $Minor, $Patch, $Prerelease > "$ProjectDir\Properties\LocalAssemblyInfo.cs";

Write-Host "Generated local assembly info.";

# Restore CWD
Pop-Location
[Environment]::CurrentDirectory = $PWD