param([string] $service = "",
      [string] $process = "",
      [string] $ready = "",
      [string] $running = "")

if (-Not (Test-Path $ready)) { Exit }

Stop-Service -Name $service

while (Get-Process $process -ErrorAction SilentlyContinue)
{
    Start-Sleep -Seconds 1
}

Copy-Item -Force -Recurse $ready -Destination $running
Remove-Item -Force -Recurse $ready
Start-Service -Name $service