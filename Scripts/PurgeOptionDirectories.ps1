$dict = @{
    1 = "F"
    2 = "G"
    3 = "H"
    4 = "J"
    5 = "K"
    6 = "M"
    7 = "N"
    8 = "Q"
    9 = "U"
    10 = "V"
    11 = "X"
    12 = "Z"
}

$monthSign = $dict[(Get-Date).AddMonths(-2).Month]
#$yearSign = @((Get-Date).AddMonths(-2).Year.ToString() | Select-Object @{Label = "Value"; Expression = {$_.Substring($_.Length - 1, 1)}}).Value
$yearSign = (Get-Date).AddMonths(-2).Year.ToString()
$yearSign = $yearSign.Substring($yearSign.Length - 1, 1)

$pattern = "^WTX[12O45]" + $monthSign + $yearSign + ";"

$directories = @(Get-ChildItem "D:\Workspace" -Directory | Where-Object -FilterScript {$_.Name -Match $pattern})

foreach ($directory in $directories)
{
    #$directory.FullName
    Remove-Item -Path $directory.FullName -Recurse -Force
}