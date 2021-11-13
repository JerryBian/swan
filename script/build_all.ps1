Invoke-Expression -Command $(Join-Path $PSScriptRoot build_admin.ps1)
Write-Host "Admin built finished."

Invoke-Expression -Command $(Join-Path $PSScriptRoot build_blog.ps1)
Write-Host "Blog built finished."

Invoke-Expression -Command $(Join-Path $PSScriptRoot build_jarvis.ps1)
Write-Host "Jarvis built finished."