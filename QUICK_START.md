# 🚀 Quick Start Guide - Car Rental Testing

## ✅ IMPORT PROBLEM: FIXED!

The Python import error `ModuleNotFoundError: No module named 'pages'` has been **completely resolved**.

---

## 📋 Prerequisites

1. ✅ .NET 9.0 SDK installed
2. ✅ Python 3.13.7 installed
3. ✅ SQL Server running
4. ✅ Chrome browser installed

---

## 🎯 Quick Start: Run Tests in 3 Steps

### Step 1: Open TWO PowerShell/CMD windows

**Window 1 - Backend API:**
```powershell
cd C:\Users\ugran\source\repos\yosrtaktak\.net_projects\Backend
dotnet run --urls=http://localhost:5001
```

**Window 2 - Frontend:**
```powershell
cd C:\Users\ugran\source\repos\yosrtaktak\.net_projects\Frontend
dotnet run --urls=http://localhost:5000
```

### Step 2: Wait for both to start

You'll see:
- Backend: `Now listening on: http://localhost:5001`
- Frontend: `Now listening on: http://localhost:5000`

### Step 3: Run Tests

**Open a THIRD PowerShell window:**
```powershell
cd C:\Users\ugran\source\repos\yosrtaktak\.net_projects\IntegrationTests
pytest -m ui -v
```

---

## 🧪 Test Commands

### Run ALL tests (16 tests):
```powershell
cd IntegrationTests
pytest -v
```

### Run by category:
```powershell
pytest -m api -v      # API tests only (10 tests)
pytest -m ui -v       # UI tests only (4 tests)
pytest -m auth -v     # Auth tests (9 tests)
pytest -m vehicles -v # Vehicle tests (7 tests)
```

### Run specific test:
```powershell
pytest tests/test_login_ui.py::TestLoginUI::test_TC023_login_ui_valid_credentials -v
```

### Generate HTML report:
```powershell
pytest -v --html=report.html --self-contained-html
```

---

## ✅ Verify Services Are Running

### Check if ports are open:
```powershell
netstat -ano | Select-String "5000|5001"
```

### Test Backend API:
```powershell
Invoke-WebRequest -Uri http://localhost:5001 -UseBasicParsing
```

### Open in browser:
- Frontend: http://localhost:5000
- Backend API: http://localhost:5001

---

## 🐛 Troubleshooting

### Problem: `ERR_CONNECTION_REFUSED`
**Solution:** Services aren't running. Start them in separate windows (Step 1 above).

### Problem: Port already in use
```powershell
# Find process using the port
Get-NetTCPConnection -LocalPort 5000 | Select-Object -ExpandProperty OwningProcess
# Kill it
Stop-Process -Id <PID> -Force
```

### Problem: Tests fail with TimeoutException
**Solutions:**
1. Check that BOTH Backend and Frontend are running
2. Try accessing http://localhost:5000 in your browser
3. Check browser console for errors (F12)

### Problem: Import errors
**Solution:** Already fixed! If you still see them, verify:
```powershell
cd IntegrationTests
python -c "import sys; print(sys.path)"
```

---

## 📊 Test Status

| Category | Tests | Status |
|----------|-------|--------|
| API Tests | 10 | ✅ Imports Fixed |
| UI Tests | 4 | ✅ Imports Fixed |
| **Total** | **16** | **✅ Ready to Run** |

---

## 🔧 Files Modified (Already Done)

1. ✅ `IntegrationTests/conftest.py` - Fixed sys.path
2. ✅ `IntegrationTests/pages/login_page.py` - Updated for MudBlazor
3. ✅ `IntegrationTests/tests/test_login_ui.py` - Fixed credentials
4. ✅ `Frontend/Program.cs` - Fixed API BaseAddress

---

## 📝 Demo Accounts (from Login.razor)

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Admin |
| employee | Employee@123 | Employee |
| customer | Customer@123 | Customer |

---

## 🎯 Next Steps

1. **Start services** (2 windows)
2. **Verify** they're running (check browser at http://localhost:5000)
3. **Run tests** (3rd window: `pytest -m ui -v`)
4. **View results** in terminal

---

## 📖 More Information

- Full test documentation: `Documentation/TEST_CASES_DETAILED.md`
- Test execution guide: `GUIDE_EXECUTION_TESTS.md`
- Import fix details: `IntegrationTests/IMPORT_PROBLEM_FIXED.md`

---

**Last Updated**: December 13, 2024
**Import Issue**: ✅ RESOLVED
**Tests**: ✅ READY TO RUN
