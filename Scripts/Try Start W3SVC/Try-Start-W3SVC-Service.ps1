$w3svcService = Get-Service -Name 'W3SVC'
$testPath = Test-Path $('filesystem::\\10.140.0.5\wantgoo-deploy\WantGoo2015Web\Web.config')

if ($testPath -and $w3svcService.Status -ne "Running") {
  Start-Service 'W3SVC'
}