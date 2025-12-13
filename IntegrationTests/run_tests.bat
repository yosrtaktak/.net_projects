@echo off
REM Script pour executer les tests d'integration Python
REM Systeme de Location de Voitures

echo ========================================
echo  TESTS D'INTEGRATION PYTHON
echo  pytest + Selenium + requests
echo ========================================
echo.

REM Verifier que Python est installe
python --version >nul 2>&1
if errorlevel 1 (
    echo [ERREUR] Python n'est pas installe ou n'est pas dans le PATH
    pause
    exit /b 1
)

REM Verifier que pytest est installe
python -c "import pytest" >nul 2>&1
if errorlevel 1 (
    echo [ATTENTION] pytest n'est pas installe
    echo Installation des dependances...
    pip install -r requirements.txt
    if errorlevel 1 (
        echo [ERREUR] Impossible d'installer les dependances
        pause
        exit /b 1
    )
)

echo [OK] Python et pytest sont installes
echo.

REM Verifier que les applications sont lancees
echo [IMPORTANT] Assurez-vous que:
echo   1. Backend API tourne sur http://localhost:5001
echo   2. Frontend tourne sur http://localhost:5000
echo.
set /p CONTINUE="Les applications sont-elles lancees? (O/N): "
if /i not "%CONTINUE%"=="O" (
    echo.
    echo Tests annules. Lancez d'abord les applications.
    pause
    exit /b 0
)

echo.
echo ----------------------------------------
echo  Execution des tests...
echo ----------------------------------------
echo.

REM Creer le dossier de rapports
if not exist "reports" mkdir reports
if not exist "screenshots" mkdir screenshots

REM Executer les tests
echo [1/4] Tests API d'authentification...
pytest tests/test_auth_api.py -v -m api
set TEST_AUTH=%errorlevel%

echo.
echo [2/4] Tests API vehicules...
pytest tests/test_vehicles_api.py -v -m api
set TEST_VEHICLES_API=%errorlevel%

echo.
echo [3/4] Tests UI connexion...
pytest tests/test_login_ui.py -v -m ui
set TEST_LOGIN_UI=%errorlevel%

echo.
echo [4/4] Tests UI vehicules...
pytest tests/test_vehicles_ui.py -v -m ui
set TEST_VEHICLES_UI=%errorlevel%

echo.
echo ========================================
echo  RESUME DES TESTS
echo ========================================

if %TEST_AUTH%==0 (
    echo [OK] Tests API Auth: REUSSIS
) else (
    echo [ERREUR] Tests API Auth: ECHECS
)

if %TEST_VEHICLES_API%==0 (
    echo [OK] Tests API Vehicles: REUSSIS
) else (
    echo [ERREUR] Tests API Vehicles: ECHECS
)

if %TEST_LOGIN_UI%==0 (
    echo [OK] Tests UI Login: REUSSIS
) else (
    echo [ERREUR] Tests UI Login: ECHECS
)

if %TEST_VEHICLES_UI%==0 (
    echo [OK] Tests UI Vehicles: REUSSIS
) else (
    echo [ERREUR] Tests UI Vehicles: ECHECS
)

echo.
echo ----------------------------------------

REM Generer le rapport HTML
echo Generation du rapport HTML...
pytest --html=reports/report.html --self-contained-html
if exist "reports\report.html" (
    echo [OK] Rapport genere: reports\report.html
)

echo.
echo Voulez-vous ouvrir le rapport HTML? (O/N)
set /p OPEN_REPORT=""
if /i "%OPEN_REPORT%"=="O" (
    start reports\report.html
)

echo.
pause
