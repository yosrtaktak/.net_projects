# Category Management - Quick Setup Script
# Run this after stopping the backend

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Category Management System Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if backend is running
$backendProcess = Get-Process -Name "Backend" -ErrorAction SilentlyContinue
if ($backendProcess) {
    Write-Host "ERROR: Backend is currently running!" -ForegroundColor Red
    Write-Host "Please stop the backend before running this script." -ForegroundColor Yellow
    Write-Host "Press any key to exit..." -ForegroundColor Yellow
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

Write-Host "Step 1: Creating database migration..." -ForegroundColor Yellow
Set-Location Backend

try {
    # Create migration
    Write-Host "Creating AddCategoriesTable migration..." -ForegroundColor Cyan
    dotnet ef migrations add AddCategoriesTable
    
    if ($LASTEXITCODE -ne 0) {
        throw "Migration creation failed"
    }
    
    Write-Host "? Migration created successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Apply migration
    Write-Host "Step 2: Applying migration to database..." -ForegroundColor Yellow
    dotnet ef database update
    
    if ($LASTEXITCODE -ne 0) {
        throw "Database update failed"
    }
    
    Write-Host "? Database updated successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Build Backend
    Write-Host "Step 3: Building Backend..." -ForegroundColor Yellow
    dotnet build
    
    if ($LASTEXITCODE -ne 0) {
        throw "Backend build failed"
    }
    
    Write-Host "? Backend built successfully!" -ForegroundColor Green
    Write-Host ""
    
    # Build Frontend
    Set-Location ..\Frontend
    Write-Host "Step 4: Building Frontend..." -ForegroundColor Yellow
    dotnet build
    
    if ($LASTEXITCODE -ne 0) {
        throw "Frontend build failed"
    }
    
    Write-Host "? Frontend built successfully!" -ForegroundColor Green
    Write-Host ""
    
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "? SETUP COMPLETED SUCCESSFULLY!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor Cyan
    Write-Host "1. Start the Backend (dotnet run in Backend folder)" -ForegroundColor White
    Write-Host "2. Start the Frontend (dotnet run in Frontend folder)" -ForegroundColor White
    Write-Host "3. Login as Admin" -ForegroundColor White
    Write-Host "4. Navigate to /admin/categories" -ForegroundColor White
    Write-Host ""
    Write-Host "Default Admin Credentials:" -ForegroundColor Yellow
    Write-Host "Username: admin" -ForegroundColor White
    Write-Host "Password: Admin@123" -ForegroundColor White
    Write-Host ""
    
} catch {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "? SETUP FAILED" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Make sure SQL Server is running" -ForegroundColor White
    Write-Host "2. Check connection string in appsettings.json" -ForegroundColor White
    Write-Host "3. Ensure no other process is using the database" -ForegroundColor White
    Write-Host ""
    Set-Location ..
    exit 1
}

Set-Location ..
Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
