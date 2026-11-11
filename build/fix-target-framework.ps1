# scripts/fix-target-framework.ps1
# AppVeyor: temporarily downgrade net11.0 -> net10.0 before build

$ErrorActionPreference = "Stop"

# 1) cd to the clone folder (APPVEYOR_BUILD_FOLDER is the clone dir)
Set-Location $env:APPVEYOR_BUILD_FOLDER
Write-Host "Working dir: $(Get-Location)"

$proj1 = Join-Path $env:APPVEYOR_BUILD_FOLDER "src\JinianNet.JNTemplate\JinianNet.JNTemplate.csproj"
$proj2 = Join-Path $env:APPVEYOR_BUILD_FOLDER "src\JinianNet.JNTemplate.Test\JinianNet.JNTemplate.Test.csproj"

# Use string.Replace (literal, no regex) to avoid accidental over-replacement.
$utf8NoBom = New-Object System.Text.UTF8Encoding($false)

# 2) JinianNet.JNTemplate.csproj: "net11.0;net10.0" -> "net10.0"
$c1 = [System.IO.File]::ReadAllText($proj1)
$c1 = $c1.Replace("net11.0;net10.0", "net10.0")
[System.IO.File]::WriteAllText($proj1, $c1, $utf8NoBom)
Write-Host "Patched: $proj1"

# 3) JinianNet.JNTemplate.Test.csproj: "net11.0" -> "net10.0"
$c2 = [System.IO.File]::ReadAllText($proj2)
$c2 = $c2.Replace("net11.0", "net10.0")
[System.IO.File]::WriteAllText($proj2, $c2, $utf8NoBom)
Write-Host "Patched: $proj2"

Write-Host "Done. Target frameworks are now .NET 10 only."