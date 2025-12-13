# Start Backend and Frontend services for testing
# Run this script from the solution root directory

$ErrorActionPreference = "Continue"

Write-Host "=======================================" -ForegroundColor Cyan
Write-Host "  Car Rental System - Service Starter  " -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host ""

# Get the script directory (solution root)
$SolutionRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

# Define paths
$BackendPath = Join-Path $SolutionRoot "Backend"
$FrontendPath = Join-Path $SolutionRoot "Frontend"

# Check if directories exist
if (-not (Test-Path $BackendPath)) {
    Write-Host "❌ Backend directory not found at: $BackendPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $FrontendPath)) {
    Write-Host "❌ Frontend directory not found at: $FrontendPath" -ForegroundColor Red
    exit 1
}

# Kill any existing processes on ports 5001 and 5173
Write-Host "🔄 Checking for existing services on ports 5001 and 5173..." -ForegroundColor Yellow
$existingProcesses = Get-NetTCPConnection -LocalPort 5001,5173 -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -Unique
if ($existingProcesses) {
    Write-Host "   Stopping existing processes..." -ForegroundColor Yellow
    foreach ($pid in $existingProcesses) {
        try {
            if ($pid -ne 0) {
                Stop-Process -Id $pid -Force -ErrorAction SilentlyContinue
                Write-Host "   ✓ Stopped process $pid" -ForegroundColor Gray
            }
        } catch {}
    }
    Start-Sleep -Seconds 2
}

# Start Backend API
Write-Host ""
Write-Host "🚀 Starting Backend API (Port 5001)..." -ForegroundColor Green
$backendJob = Start-Process -FilePath "dotnet" `
    -ArgumentList "run --no-launch-profile" `
    -WorkingDirectory $BackendPath `
    -WindowStyle Normal `
    -PassThru

Write-Host "   Backend API starting (PID: $($backendJob.Id))..." -ForegroundColor Gray

# Wait for backend to be ready
Write-Host "   Waiting for Backend API to respond..." -ForegroundColor Yellow
$maxAttempts = 30
$attempt = 0
$backendReady = $false

while ($attempt -lt $maxAttempts -and -not $backendReady) {
    $attempt++
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5001" -UseBasicParsing -TimeoutSec 2 -ErrorAction SilentlyContinue
        $backendReady = $true
        Write-Host "   ✓ Backend API is ready!" -ForegroundColor Green
    } catch {
        Write-Host "   . Attempt $attempt/$maxAttempts..." -ForegroundColor Gray
        Start-Sleep -Seconds 1
    }
}

if (-not $backendReady) {
    Write-Host "   ⚠️  Backend API did not respond within 30 seconds" -ForegroundColor Yellow
    Write-Host "   ℹ️  It may still be starting up..." -ForegroundColor Gray
}

# Start Frontend
Write-Host ""
Write-Host "🎨 Starting Frontend Blazor App (Port 5173)..." -ForegroundColor Green
$frontendJob = Start-Process -FilePath "dotnet" `
    -ArgumentList "run" `
    -WorkingDirectory $FrontendPath `
    -WindowStyle Normal `
    -PassThru

Write-Host "   Frontend starting (PID: $($frontendJob.Id))..." -ForegroundColor Gray

# Wait for frontend to be ready
Write-Host "   Waiting for Frontend to respond..." -ForegroundColor Yellow
$attempt = 0
$frontendReady = $false

while ($attempt -lt $maxAttempts -and -not $frontendReady) {
    $attempt++
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5173" -UseBasicParsing -TimeoutSec 2 -ErrorAction SilentlyContinue
        $frontendReady = $true
        Write-Host "   ✓ Frontend is ready!" -ForegroundColor Green
    } catch {
        Write-Host "   . Attempt $attempt/$maxAttempts..." -ForegroundColor Gray
        Start-Sleep -Seconds 1
    }
}

if (-not $frontendReady) {
    Write-Host "   ⚠️  Frontend did not respond within 30 seconds" -ForegroundColor Yellow
    Write-Host "   ℹ️  It may still be starting up..." -ForegroundColor Gray
}

# Summary
Write-Host ""
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host "           Services Status              " -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Backend API:  http://localhost:5001" -ForegroundColor $(if($backendReady){"Green"}else{"Yellow"})
Write-Host "Frontend:     http://localhost:5173" -ForegroundColor $(if($frontendReady){"Green"}else{"Yellow"})
Write-Host ""
Write-Host "To run tests:" -ForegroundColor Cyan
Write-Host "  cd IntegrationTests" -ForegroundColor White
Write-Host "  pytest -m ui -v" -ForegroundColor White
Write-Host ""
Write-Host "To stop services, close the terminal windows or press Ctrl+C" -ForegroundColor Gray
Write-Host ""
