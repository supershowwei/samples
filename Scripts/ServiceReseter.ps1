param([string] $service = "",
      [string] $process = "",
      [string] $ready = "",
      [string] $running = "")

Get-Service $service | Stop-Service

while (Get-Process $process -ErrorAction SilentlyContinue)
{
    Start-Sleep -Seconds 1
}

if (Test-Path $ready)
{
	Copy-Item -Force -Recurse $ready -Destination $running
	Remove-Item -Force -Recurse $ready
}

Get-Service $service | Start-Service