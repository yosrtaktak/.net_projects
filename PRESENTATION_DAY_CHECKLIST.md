# ?? Presentation Day - Quick Setup Guide
## Car Rental Management System

---

## ? Timeline: 30 Minutes Before Presentation

### Step 1: System Check (5 minutes)

#### Check Database
```powershell
# Open SQL Server Management Studio or Azure Data Studio
# Connect to your database
# Run this query to check data:

SELECT 'Vehicles' as Table_Name, COUNT(*) as Count FROM Vehicles
UNION ALL
SELECT 'Users', COUNT(*) FROM AspNetUsers
UNION ALL
SELECT 'Rentals', COUNT(*) FROM Rentals
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'Maintenances', COUNT(*) FROM Maintenances
UNION ALL
SELECT 'VehicleDamages', COUNT(*) FROM VehicleDamages;

-- Expected Results:
-- Vehicles: 10-15
-- Users: 10-15
-- Rentals: 15-20
-- Categories: 5-8
-- Maintenances: 5-10
-- VehicleDamages: 3-5
```

#### Verify Connection String
```json
// Backend/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CarRentalDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Step 2: Start Backend (3 minutes)

```powershell
# Navigate to Backend folder
cd Backend

# Restore packages (if needed)
dotnet restore

# Build
dotnet build

# Run
dotnet run

# Expected output:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: https://localhost:7095
#       Now listening on: http://localhost:5095
```

**? Backend Running Checklist:**
- [ ] No compilation errors
- [ ] Listening on HTTPS and HTTP
- [ ] Swagger UI accessible at https://localhost:7095/swagger
- [ ] Database connection successful

### Step 3: Start Frontend (3 minutes)

```powershell
# Open new terminal
# Navigate to Frontend folder
cd Frontend

# Restore packages (if needed)
dotnet restore

# Build
dotnet build

# Run
dotnet run

# Expected output:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: https://localhost:7244
```

**? Frontend Running Checklist:**
- [ ] No compilation errors
- [ ] Application loads in browser
- [ ] No console errors
- [ ] CSS styles loading correctly

### Step 4: Test User Accounts (5 minutes)

#### Test Account 1: Admin
```
Email: admin@carental.com
Password: Admin123!
Role: Admin

Expected Features:
? Dashboard with all statistics
? Manage Vehicles
? Manage Rentals
? View Customers
? Manage Employees
? Manage Categories
? Access Reports
```

#### Test Account 2: Employee
```
Email: employee@carental.com
Password: Employee123!
Role: Employee

Expected Features:
? Dashboard
? Manage Vehicles
? Manage Rentals
? View Customers
? Maintenances
? Damages
```

#### Test Account 3: Customer
```
Email: customer@carental.com
Password: Customer123!
Role: Customer

Expected Features:
? Browse Vehicles
? Make Booking
? My Rentals
? Profile
? Report Damage
```

### Step 5: Verify Key Features (10 minutes)

#### Feature 1: Vehicle Management (Admin/Employee)
- [ ] Navigate to Manage Vehicles
- [ ] Filter by status (All, Available, Rented, Maintenance)
- [ ] Click "Add Vehicle" - modal opens
- [ ] View vehicle details
- [ ] Edit vehicle works
- [ ] Delete confirmation appears

#### Feature 2: Booking Flow (Customer)
- [ ] Browse vehicles page loads
- [ ] Filter by category works
- [ ] Click "Rent Now" on available vehicle
- [ ] Date picker appears
- [ ] Price calculation works
- [ ] Booking confirmation appears
- [ ] New rental appears in "My Rentals"

#### Feature 3: Rental Management (Employee)
- [ ] Navigate to Manage Rentals
- [ ] Create new rental
- [ ] Calculate price with different strategies
- [ ] Update rental status
- [ ] Complete rental

#### Feature 4: Reports (Admin)
- [ ] Navigate to Reports
- [ ] Dashboard report loads
- [ ] Monthly revenue chart displays
- [ ] Vehicle utilization report shows data
- [ ] Date filters work

#### Feature 5: Category Management (Admin)
- [ ] Navigate to System Management > Categories
- [ ] View all categories
- [ ] Create new category
- [ ] Edit category
- [ ] Toggle active/inactive
- [ ] Delete category

### Step 6: Browser Preparation (2 minutes)

```powershell
# Clear browser cache and cookies
# Open these tabs in order:

Tab 1: https://localhost:7244 (Login page)
Tab 2: https://localhost:7095/swagger (API documentation)
Tab 3: SQL Server Management Studio (Database view)
Tab 4: Visual Studio (Code view)
```

**Browser Settings:**
- [ ] Zoom at 100%
- [ ] Clear console (F12)
- [ ] Disable browser extensions
- [ ] Full screen mode ready (F11)

### Step 7: Visual Studio Preparation (2 minutes)

**Files to Have Open:**
```
1. Backend/Controllers/VehiclesController.cs
   (Show API endpoint implementation)

2. Backend/Infrastructure/Repositories/VehicleRepository.cs
   (Show Repository pattern)

3. Backend/Application/Services/PricingStrategies/
   (Show Strategy pattern)

4. Frontend/Pages/ManageVehicles.razor
   (Show Blazor component)

5. Frontend/Services/ApiService.cs
   (Show API service)

6. Backend/Program.cs
   (Show JWT configuration)
```

---

## ?? Demo Flow Checklist

### Part 1: Introduction (2 min)
- [ ] Welcome and introduce project
- [ ] Show loading screen
- [ ] Explain technology stack
- [ ] Show architecture diagram (whiteboard/slide)

### Part 2: Admin Demo (6 min)
- [ ] Login as admin
- [ ] Tour dashboard statistics
- [ ] Manage Vehicles:
  - [ ] Show filter tabs
  - [ ] Add new vehicle
  - [ ] Edit vehicle
  - [ ] Delete vehicle (show confirmation)
- [ ] Category Management:
  - [ ] View categories
  - [ ] Create category
  - [ ] Toggle active status
- [ ] View Reports:
  - [ ] Dashboard report
  - [ ] Monthly revenue
  - [ ] Vehicle utilization

### Part 3: Employee Demo (4 min)
- [ ] Logout admin, login as employee
- [ ] Show different navigation
- [ ] Rental Management:
  - [ ] Create new rental
  - [ ] Show price calculation strategies
  - [ ] Update rental status
- [ ] Maintenance:
  - [ ] Schedule maintenance
  - [ ] Show vehicle status change

### Part 4: Customer Demo (3 min)
- [ ] Logout employee, login as customer
- [ ] Browse Vehicles:
  - [ ] Show category filters
  - [ ] View vehicle details
- [ ] Make Booking:
  - [ ] Select dates
  - [ ] Calculate price
  - [ ] Confirm booking
- [ ] My Rentals:
  - [ ] View active rentals
  - [ ] Show rental history

### Part 5: Technical Deep Dive (4 min)
- [ ] Show Swagger API documentation
- [ ] Execute test API call
- [ ] Show database in SSMS:
  - [ ] Table structure
  - [ ] Relationships
  - [ ] Sample data
- [ ] Show code in Visual Studio:
  - [ ] Repository pattern
  - [ ] Strategy pattern
  - [ ] JWT configuration
  - [ ] Blazor component

### Part 6: Q&A (5+ min)
- [ ] Answer questions confidently
- [ ] Refer to code if needed
- [ ] Use backup screenshots if demo fails

---

## ?? Troubleshooting Quick Fixes

### Issue 1: Backend Won't Start
```powershell
# Check port availability
netstat -ano | findstr :7095
netstat -ano | findstr :5095

# Kill process if needed
taskkill /PID <process_id> /F

# Or change port in launchSettings.json
```

### Issue 2: Database Connection Failed
```powershell
# Test connection
sqlcmd -S localhost -E -Q "SELECT 1"

# If fails, check:
# 1. SQL Server service running
# 2. Connection string correct
# 3. Database exists

# Recreate database if needed
cd Backend
dotnet ef database drop -f
dotnet ef database update
```

### Issue 3: Frontend Won't Load
```powershell
# Clear bin/obj folders
Remove-Item -Recurse -Force bin,obj

# Rebuild
dotnet build
dotnet run

# Check browser console (F12) for errors
```

### Issue 4: API Calls Failing (CORS)
```csharp
// Backend/Program.cs
// Ensure CORS is configured:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("https://localhost:7244")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

app.UseCors("AllowFrontend");
```

### Issue 5: JWT Authentication Not Working
```powershell
# Check JWT configuration in appsettings.json
# Verify token in browser:
# F12 > Application > Local Storage
# Should see "authToken"

# Try logout and login again
```

---

## ?? Backup Plans

### Plan A: Screenshots
**If demo completely fails, show these screenshots:**

1. Login Screen
2. Admin Dashboard
3. Manage Vehicles Grid
4. Add Vehicle Modal
5. Category Management
6. Rental Creation
7. Reports Dashboard
8. Customer Browse Vehicles
9. Booking Flow
10. Database Schema

**Location:** Create a folder "Presentation-Screenshots" with high-quality screenshots

### Plan B: Video Recording
**Record a 10-minute demo video:**
- All user roles
- Key features
- Code walkthrough
- Store in "Presentation-Video" folder

### Plan C: API Demo via Swagger
**If frontend fails:**
1. Open Swagger UI
2. Authenticate using /api/auth/login
3. Copy JWT token
4. Use "Authorize" button to add token
5. Execute API calls:
   - GET /api/vehicles
   - POST /api/rentals
   - GET /api/reports/dashboard

---

## ?? Success Indicators

### Green Flags ?
- Backend starts without errors
- Frontend loads in < 5 seconds
- All three test logins work
- API calls return data
- No console errors
- Swagger UI accessible
- Database queries execute
- Visual animations smooth
- Responsive design works

### Red Flags ? (Need to Fix)
- Compilation errors
- 500 Internal Server Error responses
- Database connection timeout
- JWT token not generated
- CORS errors in console
- Blank pages
- Missing data in database
- Slow page loads (> 10 seconds)

---

## ?? 5 Minutes Before Presentation

### Final Checklist
```
? Backend running - no errors
? Frontend running - loads properly
? All 3 accounts tested successfully
? Database has sample data
? Browser tabs ready
? Visual Studio with code open
? SSMS with database open
? Backup screenshots accessible
? Architecture diagram ready
? Notes printed/accessible
? Water bottle nearby
? Phone on silent
? Confident and ready!
```

### Mental Preparation
1. **Take a deep breath** ?????
2. **Review key talking points**
3. **Visualize successful demo**
4. **Remember: You know this project best!**
5. **Stay calm if something goes wrong**

---

## ?? Confidence Boosters

**Remember:**
- You built this entire system from scratch
- You understand every line of code
- You've solved complex problems
- You've implemented modern best practices
- This is YOUR project - own it!

**If something goes wrong:**
- Stay calm and composed
- Explain what should happen
- Use backup plan (screenshots/video)
- Show your problem-solving skills
- Professors appreciate seeing how you handle issues

---

## ?? After Presentation

### Immediate Actions
- [ ] Thank the audience
- [ ] Save any modified demo data
- [ ] Backup the entire project
- [ ] Note questions you couldn't answer
- [ ] Reflect on what went well

### Follow-up
- [ ] Research any unanswered questions
- [ ] Send thank you email if appropriate
- [ ] Update GitHub repository
- [ ] Add presentation to portfolio
- [ ] Celebrate your achievement! ??

---

## ?? Emergency Contacts

**Before presentation, have these ready:**
- [ ] University IT support number
- [ ] Backup laptop ready
- [ ] Project files on USB drive
- [ ] Project URL on GitHub
- [ ] Supervisor's contact info

---

## ?? Final Words

You've built an impressive, production-quality application that demonstrates real-world software engineering skills. Trust in your preparation, stay confident, and remember that even if the demo has hiccups, your knowledge and ability to discuss the technical implementation is what truly matters.

**You've got this!** ????

Good luck with your presentation!

---

*Print this checklist and keep it handy during setup. Check off items as you complete them.*
