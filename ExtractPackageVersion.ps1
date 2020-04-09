Param(
    [string] [Parameter(Mandatory = $true)] $projectName
)

$xml = [Xml] (Get-Content .\$projectName)
$version = $xml.Project.PropertyGroup.Version

Write-Host $version

if($version) { 
    $majorVersion, $minorVersion, $patchVersion = $version.ToString().split('.')

	Write-Host "##vso[task.setvariable variable=MajorVersion;]$majorVersion"
	Write-Host "##vso[task.setvariable variable=MinorVersion;]$minorVersion"
	Write-Host "##vso[task.setvariable variable=PatchVersion;]$patchVersion"
}

