# Quick Test Guide - Vehicle History Feature 🧪

## Understanding the Code First Approach

This project uses **Entity Framework Core Code First** approach. This means:
- Database schema is defined in C# entity classes (not directly in SQL)
- EF migrations create and update the database based on code changes
- Seed data is configured in `CarRentalDbContext.cs` and `VehicleHistorySeeder.cs`

---

## ✅ Current Status

The vehicle history feature is **fully configured** in the code:
- ✅ All entity classes exist (`Rental`, `Maintenance`, `VehicleDamage`)
- ✅ Database relationships are configured in `CarRentalDbContext`  
- ✅ Seed data is configured in `VehicleHistorySeeder.cs`
- ✅ All migrations have been applied

**However**, the seed data might not be in your database yet if you created the database before the seeder was added.

---

## 🎯 Quick Setup (Choose ONE Method)

### Option 1: Fresh Database (RECOMMENDED - Easiest) ⚡

If you can recreate your database, this is the simplest approach:

```bash
cd Backend

# Drop the existing database and recreate with all seed data
dotnet ef database drop --force
dotnet ef database update

# Start the backend
dotnet run
```

This will create a fresh database with all the test data already included!

---

### Option 2: Run SQL Script (Keep Existing Data) 📝

If you want to keep your existing database data:

#### Using PowerShell Script (Automated):
```powershell
cd Backend\Scripts
.\InsertTestData.ps1
```

#### Using SQL Server Management Studio (Manual):
1. Open SSMS and connect to your server
2. Open: `Backend\Scripts\SeedVehicleHistoryData.sql`
3. Make sure you're connected to the `CarRental` database
4. Execute the script (F5)

#### Using Command Line:
```bash
sqlcmd -S localhost -d CarRental -E -i Backend\Scripts\SeedVehicleHistoryData.sql
```

---

### Option 3: Create New Migration (For Existing DB) 🔄

If the seed data config was added after your database was created:

```bash
cd Backend

# Create a migration for the seed data
dotnet ef migrations add SeedVehicleHistoryData

# Apply the migration
dotnet ef database update
```

**Note**: EF Core automatically detects seed data configured with `HasData()` and creates INSERT statements in migrations.

---

## 🧪 Testing the History Feature

Once data is seeded:

1. **Start the Backend**
   ```bash
   cd Backend
   dotnet run
   ```

2. **Start the Frontend** (in a new terminal)
   ```bash
   cd Frontend
   dotnet run
   ```

3. **Login as Admin**
   - Navigate to: `http://localhost:5001` (or your frontend port)
   - Username: `admin`
   - Password: `Admin@123`

4. **Access Vehicle History**
   - Go to "Manage Vehicles" page
   - Find the **Toyota Corolla** card
   - Click the **🕒 History** button

5. **Explore the History Page**
   - View 4 summary cards showing totals
   - Browse through 4 tabs:
     - **Rental History** - 4 completed rentals
     - **Maintenance History** - 4 maintenance records
     - **Damage History** - 3 damage reports
     - **Mileage Evolution** - Complete timeline

---

## 📊 Expected Test Data

For **Toyota Corolla (Vehicle ID: 1)**:

| Category | Count | Details |
|----------|-------|---------|
| **Rentals** | 4 | All completed, Jan-Apr 2024 |
| **Maintenance** | 4 | 3 completed, 1 scheduled |
| **Damages** | 3 | 2 repaired, 1 under repair |
| **Mileage** | 16,000 km | Increased from 15,000 km |
| **Total Distance** | 1,000 km | Across all rentals |

---

## 🔍 Verify Data Was Inserted

Run this SQL query to check:

```sql
-- Check rental records
SELECT COUNT(*) as RentalCount FROM Rentals WHERE VehicleId = 1;

-- Check maintenance records
SELECT COUNT(*) as MaintenanceCount FROM Maintenances WHERE VehicleId = 1;

-- Check damage records
SELECT COUNT(*) as DamageCount FROM VehicleDamages WHERE VehicleId = 1;

-- Check vehicle mileage
SELECT Mileage FROM Vehicles WHERE Id = 1;
```

**Expected Results:**
- RentalCount: 4
- MaintenanceCount: 4
- DamageCount: 3
- Mileage: 16000

---

## 🛠️ Troubleshooting

### Issue: "No history available" message

**Possible Causes:**
1. Seed data wasn't inserted into database
2. Database was created before seeder was configured
3. Backend is not finding the records

**Solutions:**
1. Check if data exists using the SQL query above
2. If no data: Choose one of the setup options (1, 2, or 3) above
3. Restart the backend after inserting data

---

### Issue: "Access denied" or page doesn't load

**Solution:**
- Make sure you're logged in as **Admin**
- Test account: username `admin`, password `Admin@123`
- Clear browser cache and cookies if needed

---

### Issue: Migration errors

**Error:** "Migration already applied"
```bash
# Remove the last migration
dotnet ef migrations remove

# Create a new one
dotnet ef migrations add SeedVehicleHistoryData
dotnet ef database update
```

**Error:** "Cannot access file Backend.exe"
```bash
# Stop the backend first (Ctrl+C in the terminal running it)
# Or kill the process:
taskkill /F /IM Backend.exe

# Then run your migration commands
```

---

### Issue: SQL Script fails with duplicate key errors

**Solution:** The data already exists! Check with:
```sql
SELECT * FROM Rentals WHERE VehicleId = 1;
SELECT * FROM Maintenances WHERE VehicleId = 1;
SELECT * FROM VehicleDamages WHERE VehicleId = 1;
```

If you see records but the frontend shows "No history", restart the backend.

---

## 🧹 Clean Up Test Data

To remove test data and start fresh:

```sql
-- Remove all test history for Vehicle 1
DELETE FROM VehicleDamages WHERE VehicleId = 1;
DELETE FROM Maintenances WHERE VehicleId = 1;
DELETE FROM Rentals WHERE VehicleId = 1;
UPDATE Vehicles SET Mileage = 15000 WHERE Id = 1;
```

---

## 📚 Understanding the Code Structure

### Entities (Domain Models):
- `Vehicle.cs` - Vehicle information with navigation properties
- `Rental.cs` - Rental records with mileage tracking
- `Maintenance.cs` - Maintenance schedules and history
- `VehicleDamage.cs` - Damage reports and repairs

### DbContext:
- `CarRentalDbContext.cs` - Configures database schema and relationships
- Calls `modelBuilder.SeedVehicleHistory()` to include test data

### Seeder:
- `VehicleHistorySeeder.cs` - Defines seed data using `HasData()`
- EF Core automatically generates INSERT statements in migrations

### Migrations:
- `InitialCreate` - Creates all tables
- `AddVehicleDamageTable` - Adds VehicleDamages table
- Future migrations will include seed data if you recreate them

---

## 🎉 Success Checklist

Before marking this as complete, verify:

- [ ] Backend starts without errors
- [ ] Frontend starts and loads
- [ ] Can login as admin
- [ ] Manage Vehicles page shows Toyota Corolla
- [ ] History button is visible and clickable
- [ ] History page loads with data
- [ ] All 4 tabs show correct information
- [ ] Summary cards display accurate counts
- [ ] Mileage timeline is visible
- [ ] No console errors (F12 in browser)
- [ ] No backend errors in terminal

---

## 📖 Additional Resources

- **Feature Documentation**: `VEHICLE_HISTORY_FEATURE.md`
- **Vehicle Status Guide**: `VEHICLE_STATUS_GUIDE.md`
- **SQL Seed Script**: `Backend/Scripts/SeedVehicleHistoryData.sql`
- **PowerShell Seeder**: `Backend/Scripts/InsertTestData.ps1`

---

**Happy Testing!** 🚗💨

If you encounter any other issues, check:
1. Backend console for error messages
2. Browser console (F12) for frontend errors
3. Database connection in `appsettings.json`
4. That your database server is running
