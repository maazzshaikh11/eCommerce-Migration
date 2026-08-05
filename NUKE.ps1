# NUKE build system entry (PowerShell)
# Delegates to the Makefile for common tasks.
# Usage: .\NUKE.ps1 <target>  (e.g., .\NUKE.ps1 build)

if ($args.Count -eq 0) {
    Write-Host "Usage: .\NUKE.ps1 <target>"
    Write-Host "Targets: build, test, lint, docker-up, docker-down, clean, restore"
    exit 1
}

$target = $args[0]
Write-Host "Running: make $target"
& make $target