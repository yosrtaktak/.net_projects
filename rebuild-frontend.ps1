# Clean and Rebuild Script
# Run this to fix build issues

Write-Host "Cleaning Frontend..." -ForegroundColor Yellow
cd Frontend
dotnet clean
if ($LASTEXITCODE -ne 0) {
    Write-Host "Clean failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Building Frontend..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed! Check errors above." -ForegroundColor Red
    exit 1
}

Write-Host "`nBuild successful!" -ForegroundColor Green
Write-Host "You can now run: dotnet run" -ForegroundColor Cyan
