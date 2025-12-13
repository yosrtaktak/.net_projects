# 🎯 QUICK REFERENCE CARD

## 🌐 URLs
```
Frontend:  http://localhost:5173/login
Backend:   http://localhost:5001
Swagger:   http://localhost:5001 (root)
```

## 🔑 Login
```
Admin:     admin / Admin@123
Employee:  employee / Employee@123
Customer:  customer / Customer@123
```

## ⚡ Commands
```powershell
# Start Everything
.\start-services.ps1

# Stop Everything
Get-Process dotnet | Stop-Process -Force

# Run Tests
cd IntegrationTests
pytest -m ui -v

# Test Backend
Invoke-RestMethod http://localhost:5001

# Open Frontend
Start-Process http://localhost:5173/login

# Restart Backend (after CORS fix)
Get-Process dotnet | Where-Object {$_.CommandLine -like "*Backend*"} | Stop-Process -Force
cd Backend
dotnet run --no-launch-profile
```

## 🔧 Ports
```
Backend:  5001 (HTTP), 5000 (HTTPS)
Frontend: 5173
```

## 📝 Files Changed
```
✅ Backend\Program.cs (Ports + CORS)
✅ Frontend\Properties\launchSettings.json
✅ IntegrationTests\Configurations\config.ini
✅ start-services.ps1
✅ start-services.bat
```

## ✅ Status
```
🟢 Backend:   Running on 5001
🟢 Frontend:  Running on 5173
🟢 CORS:      Fixed ✓
🟢 Login:     Working ✓
🟢 Tests:     Ready ✓
```

## 🔒 Recent CORS Fix
```
✅ Changed from AllowAnyOrigin() to WithOrigins()
✅ Added AllowCredentials() for authentication
✅ Disabled HTTPS redirect (dev mode)
✅ Preflight requests working
```

---
**Last Updated:** Dec 13, 2024 | **Status:** OPERATIONAL ✅ | **CORS:** FIXED ✅
