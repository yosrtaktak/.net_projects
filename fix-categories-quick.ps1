# Quick Fix for Category Management
# Run this script after stopping the backend

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Category Management - Quick Fix" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if backend is running
$backendProcess = Get-Process -Name "Backend" -ErrorAction SilentlyContinue
if ($backendProcess) {
    Write-Host "ERROR: Backend is still running!" -ForegroundColor Red
    Write-Host "Please stop the backend (Ctrl+C) before running this script." -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Step 1: Creating database migration..." -ForegroundColor Yellow
Set-Location Backend

try {
    # Create migration
    Write-Host "Creating AddCategoriesTable migration..." -ForegroundColor Cyan
    dotnet ef migrations add AddCategoriesTable --verbose
    
    if ($LASTEXITCODE -ne 0) {
        throw "Migration creation failed"
    }
    
    Write-Host "? Migration created successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Apply migration
    Write-Host "Step 2: Applying migration to database..." -ForegroundColor Yellow
    dotnet ef database update --verbose
    
    if ($LASTEXITCODE -ne 0) {
        throw "Database update failed"
    }
    
    Write-Host "? Database updated successfully!" -ForegroundColor Green
    Write-Host ""
    
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "? FIX COMPLETED SUCCESSFULLY!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Cyan
    Write-Host "1. Start the Backend: dotnet run (in Backend folder)" -ForegroundColor White
    Write-Host "2. Refresh the frontend page" -ForegroundColor White
    Write-Host "3. Navigate to /admin/categories" -ForegroundColor White
    Write-Host ""
    Write-Host "The page should now load without errors!" -ForegroundColor Green
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "? FIX FAILED" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Make sure SQL Server is running" -ForegroundColor White
    Write-Host "2. Check connection string in appsettings.json" -ForegroundColor White
    Write-Host "3. Verify backend is completely stopped" -ForegroundColor White
    Write-Host ""
    Set-Location ..
    Read-Host "Press Enter to exit"
    exit 1
}

Set-Location ..
Write-Host "Would you like to start the backend now? (Y/N): " -NoNewline -ForegroundColor Yellow
$response = Read-Host

if ($response -eq "Y" -or $response -eq "y") {
    Write-Host ""
    Write-Host "Starting backend..." -ForegroundColor Cyan
    Set-Location Backend
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "dotnet run"
    Set-Location ..
    Write-Host "? Backend started in new window!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Now refresh your browser at https://localhost:5001/admin/categories" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "To start backend manually, run:" -ForegroundColor Cyan
    Write-Host "  cd Backend" -ForegroundColor White
    Write-Host "  dotnet run" -ForegroundColor White
}

Write-Host ""
Read-Host "Press Enter to exit"
