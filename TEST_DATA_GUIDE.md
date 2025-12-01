# 🧪 Test Vehicle History - Quick Start Guide

## Test Data Created for Toyota Corolla (ID: 1)

The test data includes realistic historical information for comprehensive testing of the vehicle history feature.

---

## 📊 Test Data Overview

### Vehicle: **Toyota Corolla** (ABC123)
- **Initial Mileage:** 15,000 km
- **Current Mileage:** 16,000 km
- **Total Distance Driven:** 1,000 km

### 📅 **4 Rental Records** (All Completed)

| Dates | Duration | Distance | Cost | Notes |
|-------|----------|----------|------|-------|
| Jan 15-20, 2024 | 5 days | 250 km | $175 | Regular rental |
| Feb 10-15, 2024 | 5 days | 230 km | $175 | Weekend trip |
| Mar 5-12, 2024 | 7 days | 340 km | $245 | Business trip |
| Apr 1-6, 2024 | 5 days* | 180 km | $175 | 1 day late return |

*Scheduled for 4 days, returned after 5 days

### 🔧 **4 Maintenance Records**

| Date | Type | Description | Cost | Status |
|------|------|-------------|------|--------|
| Jan 5, 2024 | Routine | Oil change & filter | $85 | ✅ Completed |
| Feb 20, 2024 | Repair | Brake pads & tire rotation | $320 | ✅ Completed |
| Mar 15, 2024 | Inspection | Annual inspection | $50 | ✅ Completed |
| May 1, 2024 | Routine | A/C service | $150 | ⏳ Scheduled |

### ⚠️ **3 Damage Reports**

| Date | Severity | Description | Repair Cost | Status |
|------|----------|-------------|-------------|--------|
| Feb 15, 2024 | Minor | Rear bumper scratch | $150 | ✅ Repaired Feb 18 |
| Mar 12, 2024 | Moderate | Driver's side door dent | $450 | ✅ Repaired Mar 20 |
| Apr 10, 2024 | Minor | Windshield chip | $200 | 🔧 Under Repair |

---

## 🚀 How to Insert Test Data

### **Option 1: Automatic (PowerShell Script)** ⚡ FASTEST

```powershell
cd Backend\Scripts
.\InsertTestData.ps1
```

This will automatically:
1. Read your database connection string
2. Execute the SQL script
3. Verify the data was inserted
4. Show a summary

---

### **Option 2: Manual SQL Execution** 📝

1. **Open SQL Server Management Studio (SSMS)**

2. **Connect to your database**
   - Server: `localhost` or your SQL Server instance
   - Database: `CarRentalDb` (or your database name)

3. **Open and execute the SQL file**
   - File: `Backend\Scripts\SeedVehicleHistoryData.sql`
   - Or copy/paste the SQL commands from the file

4. **Verify insertion**
   ```sql
   -- Should return: Rentals(4), Maintenances(4), VehicleDamages(3)
   SELECT 'Rentals' as TableName, COUNT(*) as Count FROM Rentals WHERE VehicleId = 1
   UNION ALL
   SELECT 'Maintenances', COUNT(*) FROM Maintenances WHERE VehicleId = 1
   UNION ALL
   SELECT 'VehicleDamages', COUNT(*) FROM VehicleDamages WHERE VehicleId = 1;
   ```

---

## 🧪 Testing Steps

### 1. Start the Backend
```bash
cd Backend
dotnet run
```

### 2. Start the Frontend
```bash
cd Frontend
dotnet run
```

### 3. Login as Admin
- Navigate to: `http://localhost:5001/login`
- Username: `admin`
- Password: `Admin@123`

### 4. Access Vehicle Management
- Go to: `http://localhost:5001/vehicles/manage`
- Find the **Toyota Corolla** card
- Look for the 🕒 (History) button

### 5. Click History Button
- This will navigate to: `/vehicles/1/history`
- You should see the complete vehicle history

---

## ✅ What to Verify

### Summary Dashboard
- [ ] Shows "4" total rentals
- [ ] Shows "4" total maintenances
- [ ] Shows "3" total damage reports
- [ ] Shows "1,000 km" total driven

### Rental History Tab
- [ ] Displays 4 rental cards
- [ ] Shows customer "John Doe" for all
- [ ] Shows correct dates and costs
- [ ] Shows distance driven per rental
- [ ] All have "Completed" status badge (green)

### Maintenance History Tab
- [ ] Displays 4 maintenance records
- [ ] 3 are marked "Completed" (green badge)
- [ ] 1 is marked "Scheduled" (blue badge)
- [ ] Shows costs for each
- [ ] Shows maintenance types

### Damage History Tab
- [ ] Displays 3 damage reports
- [ ] 2 are marked "Repaired" (green)
- [ ] 1 is marked "Under Repair" (blue)
- [ ] Shows severity badges (Minor/Moderate)
- [ ] Shows repair costs
- [ ] Shows who reported each damage

### Mileage Evolution Tab
- [ ] Shows current mileage: 16,000 km
- [ ] Shows total driven: 1,000 km
- [ ] Shows average per rental: 250 km
- [ ] Timeline table shows all events
- [ ] Mileage changes are calculated correctly

---

## 🎨 Visual Indicators to Check

### Status Badge Colors
- 🟢 **Green** (bg-success) = Completed/Repaired/Available
- 🔵 **Blue** (bg-info) = Scheduled/Under Repair
- 🟡 **Yellow** (bg-warning) = In Progress
- ⚫ **Gray** (bg-secondary) = Cancelled

### Severity Badge Colors (Damage)
- 🔵 **Blue** = Minor
- 🟡 **Yellow** = Moderate
- 🔴 **Red** = Major
- ⚫ **Black** = Critical

---

## 🐛 Troubleshooting

### "No history available"
**Cause:** Data not inserted  
**Fix:** Run the SQL script again and verify with:
```sql
SELECT COUNT(*) FROM Rentals WHERE VehicleId = 1;
```

### "Access denied"
**Cause:** Not logged in as admin  
**Fix:** Login with admin credentials

### "Vehicle not found"
**Cause:** Wrong vehicle ID  
**Fix:** Verify Vehicle ID 1 exists:
```sql
SELECT * FROM Vehicles WHERE Id = 1;
```

### Backend API Error
**Cause:** VehicleDamages table doesn't exist  
**Fix:** You need to create the migration first:
```bash
cd Backend
dotnet ef migrations add AddVehicleDamageEntity
dotnet ef database update
```

---

## 🧹 Clean Up Test Data

If you want to remove test data:

```sql
-- Remove all test history for Vehicle 1
DELETE FROM VehicleDamages WHERE VehicleId = 1;
DELETE FROM Maintenances WHERE VehicleId = 1;
DELETE FROM Rentals WHERE VehicleId = 1 AND CreatedAt >= '2024-01-01';
UPDATE Vehicles SET Mileage = 15000 WHERE Id = 1;
```

---

## 📸 Expected Screenshots

When testing, you should see:

1. **Management Page**: Toyota Corolla card with History button (🕒)
2. **History Page Header**: Vehicle info + 4 summary cards
3. **Tab Navigation**: 4 tabs (Rentals, Maintenance, Damage, Mileage)
4. **Data Tables**: Well-formatted data with badges and icons
5. **Responsive Layout**: Works on mobile and desktop

---

## 🎯 Success Criteria

✅ All test data is visible  
✅ No errors in console (F12)  
✅ Badges display with correct colors  
✅ Calculations are accurate (mileage, costs)  
✅ Navigation works smoothly  
✅ Data loads without lag  
✅ Admin-only access enforced  

---

## 📝 Notes

- The test data is designed to be realistic and comprehensive
- Rental dates span 4 months (Jan-Apr 2024)
- Maintenance includes both completed and scheduled items
- Damage includes various severity levels
- Mileage evolution shows clear progression

---

## 🎉 Ready to Test!

You now have everything you need to test the Vehicle History feature:
- ✅ Test data script
- ✅ Insertion tools
- ✅ Testing checklist
- ✅ Troubleshooting guide

**Happy Testing!** 🚗💨

For questions or issues, refer to `VEHICLE_HISTORY_FEATURE.md` for complete documentation.
