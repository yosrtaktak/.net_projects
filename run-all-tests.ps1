# Script d'exécution complète des tests
# Projet: Système de Location de Voitures
# Date: Décembre 2024

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  EXECUTION COMPLETE DES TESTS" -ForegroundColor Cyan
Write-Host "  Système de Location de Voitures" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Fonction pour afficher les résultats
function Show-TestResult {
    param(
        [string]$TestName,
        [bool]$Success,
        [int]$ExitCode
    )
    
    if ($Success) {
        Write-Host "? $TestName : " -NoNewline -ForegroundColor Green
        Write-Host "SUCCES" -ForegroundColor Green
    } else {
        Write-Host "? $TestName : " -NoNewline -ForegroundColor Red
        Write-Host "ECHEC (Exit Code: $ExitCode)" -ForegroundColor Red
    }
}

# Variables de comptage
$totalTests = 0
$passedTests = 0
$failedTests = 0

# ============================================
# 1. TESTS BACKEND
# ============================================
Write-Host "????????????????????????????????????????" -ForegroundColor Yellow
Write-Host "?? TESTS BACKEND" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Yellow
Write-Host ""

# Vérifier si le projet existe
if (Test-Path "Backend.Tests/Backend.Tests.csproj") {
    Write-Host "?? Tests Unitaires Backend..." -ForegroundColor Cyan
    dotnet test Backend.Tests --filter "Category=Unit" --logger "console;verbosity=minimal"
    $unitTestResult = $LASTEXITCODE -eq 0
    $totalTests++
    if ($unitTestResult) { $passedTests++ } else { $failedTests++ }
    Show-TestResult -TestName "Tests Unitaires" -Success $unitTestResult -ExitCode $LASTEXITCODE
    Write-Host ""

    Write-Host "?? Tests d'Intégration Backend..." -ForegroundColor Cyan
    dotnet test Backend.Tests --filter "Category=Integration" --logger "console;verbosity=minimal"
    $integrationTestResult = $LASTEXITCODE -eq 0
    $totalTests++
    if ($integrationTestResult) { $passedTests++ } else { $failedTests++ }
    Show-TestResult -TestName "Tests d'Intégration" -Success $integrationTestResult -ExitCode $LASTEXITCODE
    Write-Host ""

    # Génération de la couverture de code
    Write-Host "?? Génération de la couverture de code..." -ForegroundColor Cyan
    dotnet test Backend.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Rapport de couverture généré: Backend.Tests/TestResults/coverage.cobertura.xml" -ForegroundColor Green
    } else {
        Write-Host "??  Impossible de générer le rapport de couverture" -ForegroundColor Yellow
    }
    Write-Host ""
} else {
    Write-Host "??  Projet Backend.Tests introuvable" -ForegroundColor Yellow
    Write-Host ""
}

# ============================================
# 2. TESTS FRONTEND (Selenium)
# ============================================
Write-Host "????????????????????????????????????????" -ForegroundColor Yellow
Write-Host "?? TESTS FRONTEND (Selenium)" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Yellow
Write-Host ""

# Vérifier si le projet existe
if (Test-Path "Frontend.Tests/Frontend.Tests.csproj") {
    
    # Vérifier si Backend et Frontend sont lancés
    Write-Host "??  IMPORTANT: Les tests Selenium nécessitent:" -ForegroundColor Yellow
    Write-Host "   1. Backend API sur http://localhost:5001" -ForegroundColor Yellow
    Write-Host "   2. Frontend sur http://localhost:5000" -ForegroundColor Yellow
    Write-Host ""
    
    $response = Read-Host "Les applications sont-elles lancées? (O/N)"
    
    if ($response -eq "O" -or $response -eq "o") {
        Write-Host "?? Tests Système UI (Selenium)..." -ForegroundColor Cyan
        dotnet test Frontend.Tests --filter "Category=System" --logger "console;verbosity=minimal"
        $systemTestResult = $LASTEXITCODE -eq 0
        $totalTests++
        if ($systemTestResult) { $passedTests++ } else { $failedTests++ }
        Show-TestResult -TestName "Tests Système UI" -Success $systemTestResult -ExitCode $LASTEXITCODE
        Write-Host ""
        
        # Lister les screenshots générés
        $screenshots = Get-ChildItem -Path "Frontend.Tests" -Filter "screenshot_*.png" -ErrorAction SilentlyContinue
        if ($screenshots.Count -gt 0) {
            Write-Host "?? Screenshots générés:" -ForegroundColor Cyan
            foreach ($screenshot in $screenshots) {
                Write-Host "   - $($screenshot.Name)" -ForegroundColor Gray
            }
            Write-Host ""
        }
    } else {
        Write-Host "??  Tests Selenium ignorés (applications non lancées)" -ForegroundColor Yellow
        Write-Host ""
    }
} else {
    Write-Host "??  Projet Frontend.Tests introuvable" -ForegroundColor Yellow
    Write-Host ""
}

# ============================================
# 3. RÉSUMÉ DES RÉSULTATS
# ============================================
Write-Host "????????????????????????????????????????" -ForegroundColor Yellow
Write-Host "?? RÉSUMÉ DES TESTS" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Yellow
Write-Host ""

Write-Host "Total de catégories testées: " -NoNewline
Write-Host "$totalTests" -ForegroundColor Cyan

Write-Host "? Catégories réussies: " -NoNewline
Write-Host "$passedTests" -ForegroundColor Green

Write-Host "? Catégories échouées: " -NoNewline
Write-Host "$failedTests" -ForegroundColor Red

$successRate = if ($totalTests -gt 0) { [math]::Round(($passedTests / $totalTests) * 100, 2) } else { 0 }
Write-Host "?? Taux de réussite: " -NoNewline
if ($successRate -ge 80) {
    Write-Host "$successRate%" -ForegroundColor Green
} elseif ($successRate -ge 50) {
    Write-Host "$successRate%" -ForegroundColor Yellow
} else {
    Write-Host "$successRate%" -ForegroundColor Red
}

Write-Host ""

# ============================================
# 4. GÉNÉRATION DES RAPPORTS
# ============================================
Write-Host "????????????????????????????????????????" -ForegroundColor Yellow
Write-Host "?? RAPPORTS DISPONIBLES" -ForegroundColor Yellow
Write-Host "????????????????????????????????????????" -ForegroundColor Yellow
Write-Host ""

Write-Host "Documentation des tests:" -ForegroundColor Cyan
Write-Host "  ?? Plan de test: Documentation/TEST_PLAN.md" -ForegroundColor Gray
Write-Host "  ?? Cas de test détaillés: Documentation/TEST_CASES_DETAILED.md" -ForegroundColor Gray
Write-Host "  ?? Matrice de traçabilité: Documentation/TRACEABILITY_MATRIX.md" -ForegroundColor Gray
Write-Host "  ?? Guide d'exécution: Documentation/TEST_EXECUTION_GUIDE.md" -ForegroundColor Gray
Write-Host ""

if (Test-Path "Backend.Tests/TestResults/coverage.cobertura.xml") {
    Write-Host "Rapports générés:" -ForegroundColor Cyan
    Write-Host "  ?? Couverture de code: Backend.Tests/TestResults/coverage.cobertura.xml" -ForegroundColor Gray
    Write-Host ""
    
    # Proposer de générer le rapport HTML
    $generateHtml = Read-Host "Voulez-vous générer le rapport HTML de couverture? (O/N)"
    if ($generateHtml -eq "O" -or $generateHtml -eq "o") {
        Write-Host "Génération du rapport HTML..." -ForegroundColor Cyan
        
        # Vérifier si reportgenerator est installé
        $reportGenInstalled = (dotnet tool list -g | Select-String "reportgenerator")
        
        if ($reportGenInstalled) {
            reportgenerator -reports:"Backend.Tests/TestResults/coverage.cobertura.xml" -targetdir:"Backend.Tests/TestResults/CoverageReport" -reporttypes:Html
            if ($LASTEXITCODE -eq 0) {
                Write-Host "? Rapport HTML généré dans: Backend.Tests/TestResults/CoverageReport/index.html" -ForegroundColor Green
                
                # Proposer d'ouvrir le rapport
                $openReport = Read-Host "Ouvrir le rapport dans le navigateur? (O/N)"
                if ($openReport -eq "O" -or $openReport -eq "o") {
                    Start-Process "Backend.Tests/TestResults/CoverageReport/index.html"
                }
            } else {
                Write-Host "? Erreur lors de la génération du rapport HTML" -ForegroundColor Red
            }
        } else {
            Write-Host "??  ReportGenerator n'est pas installé" -ForegroundColor Yellow
            Write-Host "Pour l'installer: dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor Yellow
        }
    }
}

Write-Host ""

# ============================================
# 5. RECOMMANDATIONS
# ============================================
if ($failedTests -gt 0) {
    Write-Host "????????????????????????????????????????" -ForegroundColor Red
    Write-Host "??  RECOMMANDATIONS" -ForegroundColor Red
    Write-Host "????????????????????????????????????????" -ForegroundColor Red
    Write-Host ""
    Write-Host "Des tests ont échoué. Actions recommandées:" -ForegroundColor Yellow
    Write-Host "  1. Examiner les détails des erreurs ci-dessus" -ForegroundColor Gray
    Write-Host "  2. Vérifier les logs dans les résultats de test" -ForegroundColor Gray
    Write-Host "  3. Exécuter les tests en mode verbose:" -ForegroundColor Gray
    Write-Host "     dotnet test --logger 'console;verbosity=detailed'" -ForegroundColor DarkGray
    Write-Host "  4. Consulter les screenshots Selenium (si applicable)" -ForegroundColor Gray
    Write-Host ""
}

# ============================================
# 6. CONCLUSION
# ============================================
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
if ($failedTests -eq 0 -and $totalTests -gt 0) {
    Write-Host "? TOUS LES TESTS ONT RÉUSSI!" -ForegroundColor Green
} elseif ($totalTests -eq 0) {
    Write-Host "??  AUCUN TEST EXÉCUTÉ" -ForegroundColor Yellow
} else {
    Write-Host "? CERTAINS TESTS ONT ÉCHOUÉ" -ForegroundColor Red
}
Write-Host "????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

Write-Host "Date d'exécution: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
Write-Host ""

# Retourner le code de sortie approprié
if ($failedTests -gt 0) {
    exit 1
} else {
    exit 0
}
