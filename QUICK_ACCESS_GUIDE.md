# 🚀 QUICK START - Car Rental System

## ✅ Current Status
- **Backend API**: Running on http://localhost:5001
- **Frontend UI**: Running on http://localhost:5173
- **All port conflicts**: RESOLVED ✅

---

## 🎯 Access the Application

### Frontend (User Interface)
**URL:** http://localhost:5173

### Login Page
**URL:** http://localhost:5173/login

### Backend API (Swagger Documentation)
**URL:** http://localhost:5001

---

## 👤 Demo Accounts

### Admin Account
- **Username:** `admin`
- **Password:** `Admin@123`
- **Access:** Full system administration

### Employee Account
- **Username:** `employee`
- **Password:** `Employee@123`
- **Access:** Vehicle and rental management

### Customer Account
- **Username:** `customer`
- **Password:** `Customer@123`
- **Access:** Browse and rent vehicles

---

## 🔧 Start/Stop Services

### Start All Services
```powershell
.\start-services.ps1
```
OR
```cmd
.\start-services.bat
```

### Stop All Services
```powershell
Get-Process dotnet | Stop-Process -Force
```

### Check Service Status
```powershell
# Check Backend
Invoke-WebRequest -Uri "http://localhost:5001" -UseBasicParsing

# Check Frontend
Invoke-WebRequest -Uri "http://localhost:5173" -UseBasicParsing
```

---

## 🧪 Run Tests

### All UI Tests
```bash
cd IntegrationTests
pytest -m ui -v
```

### Login Tests Only
```bash
cd IntegrationTests
pytest -m login -v
```

### Specific Test File
```bash
cd IntegrationTests
pytest tests/test_login_ui.py -v
```

---

## 📊 Port Configuration

| Service | Port | URL |
|---------|------|-----|
| Backend API (HTTP) | 5001 | http://localhost:5001 |
| Backend API (HTTPS) | 5000 | https://localhost:5000 |
| Frontend UI | 5173 | http://localhost:5173 |

---

## 🆘 Troubleshooting

### Port Already in Use
```powershell
# Kill process on port 5001 (Backend)
Get-NetTCPConnection -LocalPort 5001 -ErrorAction SilentlyContinue | 
    Select-Object -ExpandProperty OwningProcess -Unique | 
    ForEach-Object { if ($_ -ne 0) { Stop-Process -Id $_ -Force } }

# Kill process on port 5173 (Frontend)
Get-NetTCPConnection -LocalPort 5173 -ErrorAction SilentlyContinue | 
    Select-Object -ExpandProperty OwningProcess -Unique | 
    ForEach-Object { if ($_ -ne 0) { Stop-Process -Id $_ -Force } }
```

### Clear Browser Cache
If login issues persist, clear browser cache or use incognito mode.

### Database Issues
```powershell
cd Backend
dotnet ef database update
```

---

## 📝 Configuration Files

### Backend Port
**File:** `Backend\Program.cs`
```csharp
builder.WebHost.UseUrls("https://localhost:5000", "http://localhost:5001");
```

### Frontend Port
**File:** `Frontend\Properties\launchSettings.json`
```json
"applicationUrl": "http://localhost:5173"
```

### Frontend API Connection
**File:** `Frontend\Program.cs`
```csharp
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri("http://localhost:5001/") 
});
```

### Test Configuration
**File:** `IntegrationTests\Configurations\config.ini`
```ini
baseURL = http://localhost:5173
apiURL = http://localhost:5001
```

---

## 🎬 Quick Test Flow

1. **Open Browser**: http://localhost:5173/login
2. **Login**: Use `admin` / `Admin@123`
3. **Navigate**: Explore the dashboard
4. **Test Features**: Browse vehicles, create rentals, etc.

---

## 📚 Documentation

- **Test Reports**: See `Documentation/FINAL_TEST_REPORT.md`
- **Traceability Matrix**: See `Documentation/TRACEABILITY_MATRIX.md`
- **Login Fix Details**: See `LOGIN_FIX_SUMMARY.md`

---

## ✨ Recent Fixes

✅ Backend port updated from 5002 to 5001  
✅ Frontend port changed from 5001 to 5173  
✅ Port conflict resolution  
✅ Test configuration updated  
✅ All services running correctly  

**Last Updated:** December 13, 2024
