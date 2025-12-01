# PowerShell script to seed Vehicle History test data
# This script runs the SQL seed script against your database

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host " Vehicle History Data Seeder" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Get the database connection string from appsettings.json
$appsettingsPath = Join-Path $PSScriptRoot "..\appsettings.json"

if (-Not (Test-Path $appsettingsPath)) {
    Write-Host "ERROR: appsettings.json not found at $appsettingsPath" -ForegroundColor Red
    exit 1
}

$appsettings = Get-Content $appsettingsPath | ConvertFrom-Json
$connectionString = $appsettings.ConnectionStrings.DefaultConnection

if (-Not $connectionString) {
    Write-Host "ERROR: Connection string 'DefaultConnection' not found in appsettings.json" -ForegroundColor Red
    exit 1
}

Write-Host "Connection string loaded successfully" -ForegroundColor Green
Write-Host ""

# Extract server and database name from connection string
if ($connectionString -match "Server=([^;]+).*Database=([^;]+)") {
    $server = $Matches[1]
    $database = $Matches[2]
    
    Write-Host "Server: $server" -ForegroundColor Yellow
    Write-Host "Database: $database" -ForegroundColor Yellow
    Write-Host ""
} else {
    Write-Host "WARNING: Could not parse connection string. Using default values." -ForegroundColor Yellow
    $server = "localhost"
    $database = "CarRental"
}

# Path to the SQL script
$sqlScriptPath = Join-Path $PSScriptRoot "SeedVehicleHistoryData.sql"

if (-Not (Test-Path $sqlScriptPath)) {
    Write-Host "ERROR: SQL script not found at $sqlScriptPath" -ForegroundColor Red
    exit 1
}

Write-Host "Executing SQL script..." -ForegroundColor Cyan
Write-Host ""

try {
    # Run the SQL script using sqlcmd
    $result = & sqlcmd -S $server -d $database -E -i $sqlScriptPath -b
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "SUCCESS! Test data has been seeded successfully." -ForegroundColor Green
        Write-Host ""
        Write-Host "Next steps:" -ForegroundColor Cyan
        Write-Host "1. Start the Backend: cd Backend && dotnet run" -ForegroundColor White
        Write-Host "2. Start the Frontend: cd Frontend && dotnet run" -ForegroundColor White
        Write-Host "3. Login as admin (username: admin, password: Admin@123)" -ForegroundColor White
        Write-Host "4. Go to Manage Vehicles and click the History button on Toyota Corolla" -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "ERROR: Failed to execute SQL script. Exit code: $LASTEXITCODE" -ForegroundColor Red
        Write-Host "Output: $result" -ForegroundColor Red
        Write-Host ""
        Write-Host "Trying alternative method using Invoke-Sqlcmd..." -ForegroundColor Yellow
        
        # Try using Invoke-Sqlcmd if available
        if (Get-Command Invoke-Sqlcmd -ErrorAction SilentlyContinue) {
            Invoke-Sqlcmd -ServerInstance $server -Database $database -InputFile $sqlScriptPath
            Write-Host "SUCCESS! Test data seeded using Invoke-Sqlcmd." -ForegroundColor Green
        } else {
            Write-Host "ERROR: Invoke-Sqlcmd is not available. Please install SQL Server PowerShell module:" -ForegroundColor Red
            Write-Host "Install-Module -Name SqlServer" -ForegroundColor Yellow
            Write-Host ""
            Write-Host "OR run the SQL script manually:" -ForegroundColor Yellow
            Write-Host "1. Open SQL Server Management Studio" -ForegroundColor White
            Write-Host "2. Connect to your server: $server" -ForegroundColor White
            Write-Host "3. Open the file: $sqlScriptPath" -ForegroundColor White
            Write-Host "4. Execute the script (F5)" -ForegroundColor White
        }
    }
} catch {
    Write-Host "ERROR: An exception occurred" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Please run the SQL script manually:" -ForegroundColor Yellow
    Write-Host "File location: $sqlScriptPath" -ForegroundColor White
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
