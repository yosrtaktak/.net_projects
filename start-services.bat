@echo off
echo Starting Backend API on port 5001...
cd /d "%~dp0Backend"
start "Backend API" cmd /k "dotnet run --urls=http://localhost:5001"

timeout /t 5 /nobreak

echo Starting Frontend on port 5000...
cd /d "%~dp0Frontend"  
start "Frontend Blazor" cmd /k "dotnet run --urls=http://localhost:5000"

echo.
echo Services are starting!
echo Backend API: http://localhost:5001
echo Frontend: http://localhost:5000
echo.
echo Close the CMD windows to stop the services.
pause
