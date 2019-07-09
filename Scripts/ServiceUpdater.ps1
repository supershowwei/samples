param([string] $service = "",
      [string] $process = "",
      [string] $ready = "",
      [string] $running = "")

Stop-Service -Name $service

while (Get-Process $process -ErrorAction SilentlyContinue)
{
    Start-Sleep -Seconds 1
}

Copy-Item -Force -Recurse $ready -Destination $running
Start-Service -Name $service