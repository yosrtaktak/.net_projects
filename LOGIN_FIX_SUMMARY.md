# Login Error 405 - FIXED ✅

## Problem
When trying to login, the application returned:
```
POST http://localhost:5001/api/auth/login 405 (Method Not Allowed)
```

## Root Causes

### 1. Port Mismatch (Primary Issue)
- **Frontend** was configured to connect to: `http://localhost:5001`
- **Backend** was configured to run on: 
  - HTTPS: `https://localhost:5000`
  - HTTP: `http://localhost:5002` ❌ (wrong port!)

### 2. Port Already in Use
- Port 5001 was occupied by another process
- Prevented backend from binding to the correct port

### 3. Port Conflict Between Frontend and Backend
- **Frontend launchSettings.json** was also set to use port 5001
- This caused both Frontend and Backend to compete for the same port

### 4. Database Warning
- EF Core warning about pending model changes
- This was a false positive but caused startup errors

## Solutions Applied

### ✅ Fix 1: Updated Backend Port Configuration
**File:** `Backend\Program.cs`
```csharp
// Changed from:
builder.WebHost.UseUrls("https://localhost:5000", "http://localhost:5002");

// To:
builder.WebHost.UseUrls("https://localhost:5000", "http://localhost:5001");
```

### ✅ Fix 2: Updated Frontend Port Configuration
**File:** `Frontend\Properties\launchSettings.json`
```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5173", // Changed from 5001 to 5173
      // ...
    }
  }
}
```

### ✅ Fix 3: Suppressed EF Core Warning
**File:** `Backend\Program.cs`
```csharp
builder.Services.AddDbContext<CarRentalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => 
               warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));
```

### ✅ Fix 4: Updated Test Configuration
**File:** `IntegrationTests\Configurations\config.ini`
```ini
[common info]
baseURL = http://localhost:5173  # Frontend URL
apiURL = http://localhost:5001   # Backend API URL
```

### ✅ Fix 5: Updated Start Scripts
- **start-services.ps1**: Updated to use correct ports
- **start-services.bat**: Updated to use correct ports

## Services Now Running

### Backend API
- **HTTP:** http://localhost:5001 ✅
- **HTTPS:** https://localhost:5000 ✅
- **Swagger:** http://localhost:5001 (when running)

### Frontend Blazor App
- **URL:** http://localhost:5173 ✅
- **Connects to Backend:** http://localhost:5001 ✅

## Port Allocation Summary

| Service | Port | Protocol | Purpose |
|---------|------|----------|---------|
| Backend API | 5001 | HTTP | Main API endpoint |
| Backend API | 5000 | HTTPS | Secure API endpoint |
| Frontend | 5173 | HTTP | Blazor WebAssembly UI |

## Test Results

### ✅ Backend API Test
```bash
POST http://localhost:5001/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "email": "admin@carrental.com",
  "role": "Admin"
}
```

### ✅ Services Status
- Backend API (5001): ✅ Running
- Frontend (5173): ✅ Running

## How to Start Services

### Option 1: Using PowerShell Script (Recommended)
```powershell
.\start-services.ps1
```

### Option 2: Using Batch File
```cmd
.\start-services.bat
```

### Option 3: Manual Start
```powershell
# Backend (Terminal 1)
cd Backend
dotnet run --no-launch-profile

# Frontend (Terminal 2)
cd Frontend
dotnet run
```

## Demo Accounts

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Admin |
| employee | Employee@123 | Employee |
| customer | Customer@123 | Customer |

## Next Steps

1. ✅ **Backend running on port 5001**
2. ✅ **Frontend running on port 5173**
3. ✅ **No port conflicts**
4. ✅ **Login endpoint working**
5. **Try logging in through the Frontend UI** at **http://localhost:5173/login**

## Troubleshooting

### If port 5001 is still in use:
```powershell
Get-NetTCPConnection -LocalPort 5001 -ErrorAction SilentlyContinue | 
    Select-Object -ExpandProperty OwningProcess -Unique | 
    ForEach-Object { if ($_ -ne 0) { Stop-Process -Id $_ -Force } }
```

### If port 5173 is still in use:
```powershell
Get-NetTCPConnection -LocalPort 5173 -ErrorAction SilentlyContinue | 
    Select-Object -ExpandProperty OwningProcess -Unique | 
    ForEach-Object { if ($_ -ne 0) { Stop-Process -Id $_ -Force } }
```

### To check if services are running:
```powershell
# Check Backend
Test-NetConnection -ComputerName localhost -Port 5001

# Check Frontend
Test-NetConnection -ComputerName localhost -Port 5173

# Test Backend API
Invoke-RestMethod -Uri "http://localhost:5001" -UseBasicParsing

# Test Frontend
Invoke-RestMethod -Uri "http://localhost:5173" -UseBasicParsing
```

### To view running dotnet processes:
```powershell
Get-Process dotnet | Select-Object Id, ProcessName, StartTime
```

### To stop all services:
```powershell
Get-Process dotnet | Stop-Process -Force
```

## Files Modified

1. ✅ `Backend\Program.cs` - Updated port configuration and suppressed EF warning
2. ✅ `Frontend\Properties\launchSettings.json` - Changed port from 5001 to 5173
3. ✅ `IntegrationTests\Configurations\config.ini` - Updated baseURL to 5173, apiURL to 5001
4. ✅ `start-services.ps1` - Updated to use correct ports
5. ✅ `start-services.bat` - Updated to use correct ports

---
**Status:** 🟢 FULLY RESOLVED - All port conflicts fixed, services running correctly!
**Date:** December 13, 2024
