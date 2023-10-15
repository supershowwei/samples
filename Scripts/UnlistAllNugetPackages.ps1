$packageId = Read-Host "Please enter the Package ID"
$apiKey = Read-Host "Please enter the NuGet Api Unlist Key"

$json = Invoke-WebRequest -Uri "https://api.nuget.org/v3-flatcontainer/$PackageId/index.json" | ConvertFrom-Json

foreach($version in $json.versions)
{
  Write-Host "Unlisting $packageId, Ver $version"
  dotnet nuget delete $packageId $version --source https://api.nuget.org/v3/index.json --non-interactive --api-key $apiKey
}