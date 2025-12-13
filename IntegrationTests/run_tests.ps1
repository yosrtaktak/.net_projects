# Script PowerShell pour executer les tests d'integration Python
# Systeme de Location de Voitures

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  TESTS D'INTEGRATION PYTHON" -ForegroundColor Cyan
Write-Host "  pytest + Selenium + requests" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verifier Python
try {
    $pythonVersion = python --version 2>&1
    Write-Host "? $pythonVersion" -ForegroundColor Green
} catch {
    Write-Host "? Python n'est pas installe" -ForegroundColor Red
    exit 1
}

# Verifier pytest
try {
    python -c "import pytest" 2>$null
    Write-Host "? pytest est installe" -ForegroundColor Green
} catch {
    Write-Host "? pytest n'est pas installe" -ForegroundColor Yellow
    Write-Host "Installation des dependances..." -ForegroundColor Yellow
    pip install -r requirements.txt
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "  PREREQUIS" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "Assurez-vous que:" -ForegroundColor White
Write-Host "  1. Backend API tourne sur http://localhost:5001" -ForegroundColor White
Write-Host "  2. Frontend tourne sur http://localhost:5000" -ForegroundColor White
Write-Host ""

$continue = Read-Host "Les applications sont-elles lancees? (O/N)"
if ($continue -ne "O" -and $continue -ne "o") {
    Write-Host "Tests annules." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  EXECUTION DES TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Creer les dossiers
New-Item -ItemType Directory -Force -Path "reports" | Out-Null
New-Item -ItemType Directory -Force -Path "screenshots" | Out-Null

$results = @{
    "API Auth" = $false
    "API Vehicles" = $false
    "UI Login" = $false
    "UI Vehicles" = $false
}

# Tests API Auth
Write-Host "[1/4] Tests API d'authentification..." -ForegroundColor Cyan
pytest tests/test_auth_api.py -v -m api
$results["API Auth"] = ($LASTEXITCODE -eq 0)

Write-Host ""
Write-Host "[2/4] Tests API vehicules..." -ForegroundColor Cyan
pytest tests/test_vehicles_api.py -v -m api
$results["API Vehicles"] = ($LASTEXITCODE -eq 0)

Write-Host ""
Write-Host "[3/4] Tests UI connexion..." -ForegroundColor Cyan
pytest tests/test_login_ui.py -v -m ui
$results["UI Login"] = ($LASTEXITCODE -eq 0)

Write-Host ""
Write-Host "[4/4] Tests UI vehicules..." -ForegroundColor Cyan
pytest tests/test_vehicles_ui.py -v -m ui
$results["UI Vehicles"] = ($LASTEXITCODE -eq 0)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RESUME DES TESTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

foreach ($test in $results.Keys) {
    if ($results[$test]) {
        Write-Host "? $test : REUSSIS" -ForegroundColor Green
    } else {
        Write-Host "? $test : ECHECS" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "----------------------------------------" -ForegroundColor Cyan

# Generer le rapport HTML
Write-Host "Generation du rapport HTML..." -ForegroundColor Cyan
pytest --html=reports/report.html --self-contained-html

if (Test-Path "reports\report.html") {
    Write-Host "? Rapport genere: reports\report.html" -ForegroundColor Green
    
    $openReport = Read-Host "Ouvrir le rapport HTML? (O/N)"
    if ($openReport -eq "O" -or $openReport -eq "o") {
        Start-Process "reports\report.html"
    }
}

# Afficher les screenshots
$screenshots = Get-ChildItem -Path "screenshots" -Filter "*.png" -ErrorAction SilentlyContinue
if ($screenshots.Count -gt 0) {
    Write-Host ""
    Write-Host "?? Screenshots captures: $($screenshots.Count)" -ForegroundColor Yellow
    foreach ($screenshot in $screenshots) {
        Write-Host "  - $($screenshot.Name)" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "Tests termines!" -ForegroundColor Green
