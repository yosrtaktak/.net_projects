# Guide d'Exécution des Tests - Système de Location de Voitures

## ?? Table des Matières
1. [Prérequis](#prérequis)
2. [Installation](#installation)
3. [Structure des Tests](#structure-des-tests)
4. [Exécution des Tests](#exécution-des-tests)
5. [Rapports de Tests](#rapports-de-tests)
6. [Troubleshooting](#troubleshooting)

## ?? Prérequis

### Logiciels Requis
- ? .NET 9.0 SDK ou supérieur
- ? Visual Studio 2022 ou VS Code
- ? Google Chrome (pour tests Selenium)
- ? SQL Server (LocalDB ou instance complète)

### Vérification des Prérequis
```bash
# Vérifier .NET SDK
dotnet --version

# Vérifier SQL Server
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION"
```

## ?? Installation

### 1. Restaurer les Packages NuGet
```bash
# Depuis la racine du projet
dotnet restore Backend.Tests/Backend.Tests.csproj
dotnet restore Frontend.Tests/Frontend.Tests.csproj
```

### 2. Configuration de la Base de Données de Test
```bash
# Créer une base de données de test
cd Backend
dotnet ef database update --connection "Server=(localdb)\\MSSQLLocalDB;Database=CarRentalTest;Trusted_Connection=True;"
```

### 3. Installation du ChromeDriver
Le ChromeDriver est inclus via le package NuGet `Selenium.WebDriver.ChromeDriver`.
Assurez-vous que Chrome est installé et à jour.

## ??? Structure des Tests

```
.
??? Backend.Tests/
?   ??? UnitTests/
?   ?   ??? Services/          # Tests des services métier
?   ?   ?   ??? RentalServiceTests.cs
?   ?   ??? Controllers/       # Tests des contrôleurs
?   ?       ??? VehiclesControllerTests.cs
?   ??? IntegrationTests/
?   ?   ??? API/              # Tests d'intégration API
?   ?       ??? AuthApiIntegrationTests.cs
?   ?       ??? VehiclesApiIntegrationTests.cs
?   ??? Backend.Tests.csproj
?
??? Frontend.Tests/
?   ??? Helpers/              # Classes utilitaires
?   ?   ??? SeleniumTestBase.cs
?   ??? PageObjects/          # Pattern Page Object Model
?   ?   ??? LoginPage.cs
?   ?   ??? VehiclesPage.cs
?   ??? SystemTests/
?   ?   ??? UI/               # Tests système Selenium
?   ?       ??? LoginSystemTests.cs
?   ?       ??? VehicleBrowsingSystemTests.cs
?   ??? Frontend.Tests.csproj
?
??? Documentation/
    ??? TEST_PLAN.md
    ??? TEST_CASES_DETAILED.md
    ??? TRACEABILITY_MATRIX.md
    ??? TEST_EXECUTION_GUIDE.md (ce fichier)
```

## ?? Exécution des Tests

### Tests Backend

#### Exécuter TOUS les tests backend
```bash
cd Backend.Tests
dotnet test
```

#### Exécuter par catégorie
```bash
# Tests unitaires uniquement
dotnet test --filter "Category=Unit"

# Tests d'intégration uniquement
dotnet test --filter "Category=Integration"
```

#### Exécuter un test spécifique
```bash
# Par nom de classe
dotnet test --filter "FullyQualifiedName~RentalServiceTests"

# Par TestCase ID (via Trait)
dotnet test --filter "TestCase=TC001"
```

#### Avec verbosité détaillée
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Tests Frontend (Selenium)

?? **Important**: Avant d'exécuter les tests Selenium:
1. Lancez le Backend API sur `http://localhost:5001`
2. Lancez le Frontend sur `http://localhost:5000`

```bash
# Terminal 1: Lancer le Backend
cd Backend
dotnet run

# Terminal 2: Lancer le Frontend
cd Frontend
dotnet run

# Terminal 3: Exécuter les tests Selenium
cd Frontend.Tests
dotnet test
```

#### Exécuter un test Selenium spécifique
```bash
dotnet test --filter "TestCase=TC023"
```

#### Mode Headless (sans interface graphique)
Modifiez `SeleniumTestBase.cs`:
```csharp
options.AddArgument("--headless");  // Décommenter cette ligne
```

### Exécution Rapide - Tous les Tests

Utilisez le script PowerShell fourni:
```bash
.\run-all-tests.ps1
```

Ou créez un script batch:
```batch
@echo off
echo === Exécution des Tests Backend ===
cd Backend.Tests
dotnet test --logger "console;verbosity=normal"

echo.
echo === Exécution des Tests Frontend ===
cd ..\Frontend.Tests
dotnet test --logger "console;verbosity=normal"

echo.
echo === Tests Terminés ===
pause
```

## ?? Rapports de Tests

### Génération de Rapport de Couverture de Code

#### Installation de ReportGenerator
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

#### Générer la Couverture
```bash
cd Backend.Tests

# Exécuter tests avec couverture
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Générer le rapport HTML
reportgenerator -reports:coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html

# Ouvrir le rapport
start coveragereport/index.html
```

### Rapport de Test en XML (pour CI/CD)
```bash
dotnet test --logger "trx;LogFileName=test-results.trx"
```

### Rapport de Test en HTML
```bash
# Installer package liquidtestrepors.markdown
dotnet tool install -g LiquidTestReports.Markdown

# Exécuter avec rapport
dotnet test --logger "liquid.md;LogFileName=test-report.md"
```

## ?? Screenshots des Tests Selenium

Les screenshots sont automatiquement capturés en cas d'échec:
- **Emplacement**: Racine du projet `Frontend.Tests/`
- **Format**: `screenshot_{testName}_{timestamp}.png`
- **Quand**: À chaque échec de test ou appel manuel à `TakeScreenshot()`

## ?? Troubleshooting

### Problème: ChromeDriver incompatible
**Erreur**: `session not created: This version of ChromeDriver only supports Chrome version X`

**Solution**:
```bash
# Mettre à jour le package NuGet
dotnet add Frontend.Tests package Selenium.WebDriver.ChromeDriver --version [LATEST]

# Ou télécharger manuellement depuis
# https://chromedriver.chromium.org/
```

### Problème: Backend API non accessible
**Erreur**: `Connection refused` ou `404 Not Found`

**Solution**:
1. Vérifier que le Backend tourne:
   ```bash
   curl http://localhost:5001/api/vehicles
   ```
2. Vérifier le port dans `launchSettings.json`
3. Ajuster `BaseUrl` dans `SeleniumTestBase.cs`

### Problème: Tests d'intégration échouent - Database
**Erreur**: `Cannot open database CarRental`

**Solution**:
```bash
# Recréer la base de données
cd Backend
dotnet ef database drop --force
dotnet ef database update
```

### Problème: Tests Selenium lents
**Symptômes**: Tests prennent > 30 secondes

**Solutions**:
1. Réduire les `Thread.Sleep()`:
   ```csharp
   System.Threading.Thread.Sleep(1000); // Au lieu de 5000
   ```
2. Utiliser des attentes explicites:
   ```csharp
   Wait.Until(d => d.FindElement(By.Id("element")));
   ```
3. Activer le mode headless

### Problème: Couverture de code non générée
**Erreur**: `No coverage data available`

**Solution**:
```bash
# Installer coverlet.msbuild
cd Backend.Tests
dotnet add package coverlet.msbuild

# Nettoyer et rebuilder
dotnet clean
dotnet build
dotnet test /p:CollectCoverage=true
```

## ?? Bonnes Pratiques

### 1. Isolation des Tests
- Chaque test doit être indépendant
- Utiliser `[Fact]` pour tests unitaires
- Utiliser `[Theory]` pour tests paramétrés

### 2. Nommage des Tests
```csharp
// Format recommandé:
// MethodeName_Scenario_ExpectedBehavior
public void CalculatePrice_ValidDates_ReturnsCorrectPrice() { }
```

### 3. Arrange-Act-Assert (AAA)
```csharp
[Fact]
public void Test_Example()
{
    // Arrange - Préparer les données
    var service = new MyService();
    
    // Act - Exécuter l'action
    var result = service.DoSomething();
    
    // Assert - Vérifier le résultat
    result.Should().BeTrue();
}
```

### 4. Utilisation des Traits
```csharp
[Trait("Category", "Unit")]
[Trait("TestCase", "TC001")]
[Trait("Priority", "High")]
public void MyTest() { }
```

### 5. Gestion des Ressources Selenium
```csharp
// Toujours utiliser IDisposable
public class MyTests : SeleniumTestBase
{
    public void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }
}
```

## ?? Intégration Continue (CI/CD)

### GitHub Actions Exemple
```yaml
name: Run Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Run Backend Tests
      run: dotnet test Backend.Tests --no-build --verbosity normal
    
    - name: Generate Coverage Report
      run: dotnet test Backend.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
    
    - name: Upload Coverage
      uses: codecov/codecov-action@v3
      with:
        files: ./Backend.Tests/coverage.cobertura.xml
```

## ?? Ressources Additionnelles

### Documentation Officielle
- [xUnit Documentation](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4)
- [FluentAssertions Docs](https://fluentassertions.com/)
- [Selenium WebDriver C#](https://www.selenium.dev/documentation/webdriver/)

### Tutoriels
- [Unit Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [Integration Testing in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [Page Object Pattern](https://www.selenium.dev/documentation/test_practices/encouraged/page_object_models/)

## ?? Checklist de Test

Avant de considérer les tests complets:

- [ ] Tous les tests unitaires passent (100%)
- [ ] Tous les tests d'intégration passent (100%)
- [ ] Tests Selenium exécutés avec succès
- [ ] Couverture de code > 70%
- [ ] Rapport de couverture généré
- [ ] Documentation à jour
- [ ] Screenshots des tests UI capturés
- [ ] Matrice de traçabilité complétée
- [ ] Rapport d'exécution finalisé

## ?? Support

Pour toute question ou problème:
1. Consulter cette documentation
2. Vérifier les issues GitHub du projet
3. Contacter l'équipe de test

---

**Version**: 1.0  
**Dernière mise à jour**: Décembre 2024  
**Auteurs**: Équipe Test
