# Test Backend Startup
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Backend Startup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Stop any existing backend processes
Write-Host "Stopping existing backend processes..." -ForegroundColor Yellow
Stop-Process -Name "Backend","dotnet" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Navigate to Backend directory
Set-Location "C:\Users\ugran\source\repos\yosrtaktak\.net_projects\Backend"

# Start the backend
Write-Host "Starting backend..." -ForegroundColor Yellow
Write-Host ""

$process = Start-Process -FilePath "dotnet" -ArgumentList "run" -PassThru -NoNewWindow -RedirectStandardOutput "backend-output.log" -RedirectStandardError "backend-error.log"

Write-Host "Backend starting with PID: $($process.Id)" -ForegroundColor Green
Write-Host "Waiting for startup..." -ForegroundColor Yellow

# Wait for startup
Start-Sleep -Seconds 10

# Check if process is still running
if (Get-Process -Id $process.Id -ErrorAction SilentlyContinue) {
    Write-Host "" 
    Write-Host "? Backend is running!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Checking output..." -ForegroundColor Yellow
    Write-Host ""
    
    # Show last 20 lines of output
    if (Test-Path "backend-output.log") {
        Get-Content "backend-output.log" | Select-Object -Last 20
    }
    
    Write-Host ""
    Write-Host "Check errors:" -ForegroundColor Yellow
    if (Test-Path "backend-error.log") {
        Get-Content "backend-error.log" | Select-Object -Last 10
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Backend Status" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "? Running on PID: $($process.Id)" -ForegroundColor Green
    Write-Host "?? URL: https://localhost:5000" -ForegroundColor Cyan
    Write-Host "?? Swagger: https://localhost:5000/swagger" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "To stop: Stop-Process -Id $($process.Id)" -ForegroundColor Yellow
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "? Backend failed to start!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check logs:" -ForegroundColor Yellow
    if (Test-Path "backend-error.log") {
        Get-Content "backend-error.log"
    }
}
