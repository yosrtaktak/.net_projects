# Quick Test Guide - Fixed Features

## Prerequisites
```powershell
# Terminal 1 - Start Backend
cd Backend
dotnet run

# Terminal 2 - Start Frontend  
cd Frontend
dotnet run
```

Wait for both to start, then navigate to:
- Backend: https://localhost:5000
- Frontend: https://localhost:7148

## Test 1: ManageCategories ?

### Steps:
1. Login as **Admin** (username: `admin`, password from your DB)
2. Navigate to: `https://localhost:7148/admin/categories`
3. **Expected**: Category list appears with Create button

### Test Actions:
- ? Click "Add Category" - modal should open
- ? Create a new category (e.g., "Sports Cars")
- ? Edit an existing category
- ? Toggle category active/inactive
- ? Try to delete (should warn if vehicles exist)

### Success Criteria:
- All CRUD operations work
- Success/error messages appear
- Table updates after each action

---

## Test 2: Customer in ManageRentals List ?

### Steps:
1. Login as **Admin** or **Employee**
2. Navigate to: `https://localhost:7148/rentals/manage`
3. **Expected**: List of rentals with customer names and emails

### Verify:
- ? **Customer column** shows:
  - Customer first name + last name
  - Customer email (in gray text below name)
- ? **Vehicle column** shows:
  - Brand and Model
  - Category (in gray text)
- ? All other columns display correctly (dates, status, cost)

### If Customer Shows "N/A":
This means the backend isn't including User data. Check:
```sql
-- Verify rentals have valid UserId
SELECT r.Id, r.UserId, u.FirstName, u.LastName, u.Email
FROM Rentals r
LEFT JOIN AspNetUsers u ON r.UserId = u.Id
```

---

## Test 3: Rental Details Page ? (NEW)

### Steps:
1. From ManageRentals page, click **"View Details"** on any rental
2. **Expected**: Navigate to `/rentals/{id}` with full details

### Verify Details Page Shows:
- ? **Rental #** and creation date
- ? **Status badge** with color (Green=Active, Blue=Reserved, etc.)
- ? **Customer Information Card**:
  - Name
  - Email  
  - Phone
  - Tier badge
- ? **Vehicle Information Card**:
  - Brand, Model, Year
  - Category
  - License Plate (Registration Number)
  - Daily Rate
- ? **Rental Details**:
  - Start Date
  - End Date
  - Duration (days)
  - Total Cost (large green text)
  - Actual Return Date (if completed)
  - Start/End Mileage (if available)
  - Notes (if any)

### Test Actions (Admin/Employee Only):
- ? Click **"Complete Rental"**:
  - Dialog opens
  - Enter end mileage
  - Click Complete
  - Status changes to "Completed"
- ? Click **"Cancel Rental"**:
  - Rental status changes to "Cancelled"
- ? Click **"Update Status"**:
  - Dialog opens
  - Select new status
  - Click Update
  - Status changes

### Test Back Navigation:
- ? Click "Back" button
- **Expected**: Returns to `/rentals/manage`

---

## Test 4: View Details from Multiple Entry Points

### Test Navigation Works From:
1. ? **ManageRentals** ? Click "View Details" ? Details page
2. ? **MyRentals** (Customer) ? Click "View Reports" ? Should still work
3. ? **Direct URL** ? Navigate to `/rentals/1` ? Shows details

---

## Test 5: Customer Display in API Responses

### Using Browser Dev Tools:
1. Open DevTools (F12)
2. Go to **Network** tab
3. Navigate to ManageRentals page
4. Find the request to `/api/rentals/manage`
5. Check the response JSON

### Expected Response Structure:
```json
[
  {
    "id": 1,
    "userId": "abc-123-...",
    "vehicleId": 1,
    "startDate": "2024-12-10T00:00:00",
    "endDate": "2024-12-15T00:00:00",
    "totalCost": 450.00,
    "status": "Active",
    "customer": {
      "id": "abc-123-...",
      "firstName": "John",
      "lastName": "Doe",
      "email": "john@example.com",
      "phoneNumber": "555-0100",
      "tier": "Standard"
    },
    "vehicle": {
      "id": 1,
      "brand": "Toyota",
      "model": "Camry",
      "year": 2023,
      "category": "Midsize",
      "licensePlate": "ABC123",
      "dailyRate": 45.00
    }
  }
]
```

### Key Points:
- ? `customer` property exists (not null)
- ? `customer.firstName` and `customer.lastName` are populated
- ? `vehicle.licensePlate` exists
- ? `payment` property included if payment exists

---

## Troubleshooting

### Issue: Customer Shows "N/A"
**Cause**: Rental has no associated User (UserId is invalid)

**Fix**:
```sql
-- Check for orphaned rentals
SELECT r.Id, r.UserId, u.Id as UserExists
FROM Rentals r
LEFT JOIN AspNetUsers u ON r.UserId = u.Id
WHERE u.Id IS NULL;

-- Fix if needed: Update to valid user ID
UPDATE Rentals 
SET UserId = '<valid-user-id>' 
WHERE UserId NOT IN (SELECT Id FROM AspNetUsers);
```

### Issue: "Rental Not Found" on Details Page
**Cause**: Invalid rental ID or rental doesn't exist

**Test**:
```sql
SELECT * FROM Rentals WHERE Id = 1;
```

### Issue: Actions Don't Work on Details Page
**Cause**: Not logged in as Admin/Employee

**Check**:
- Logout and login again
- Verify role is "Admin" or "Employee"
- Check browser localStorage for `userRole`

### Issue: ManageCategories Shows Empty
**Cause**: No categories in database

**Fix**:
```sql
-- Create test category
INSERT INTO Categories (Name, Description, IsActive, DisplayOrder, PriceMultiplier, CreatedAt)
VALUES ('Economy', 'Budget-friendly vehicles', 1, 0, 1.0, GETDATE());
```

---

## Quick Verification Checklist

Before considering the fix complete, verify:

- [x] Backend builds without errors
- [x] Frontend builds without errors
- [ ] ManageCategories page loads
- [ ] ManageRentals shows customer names
- [ ] Rental Details page displays correctly
- [ ] Customer information is visible in details
- [ ] Vehicle information is visible in details
- [ ] All action buttons work (Admin/Employee)
- [ ] Navigation works correctly
- [ ] API returns Customer property in responses

---

## Success Indicators

### You'll know everything works when:
1. **ManageRentals list** shows customer names (not "N/A")
2. **Rental Details page** loads with all information
3. **Customer card** shows name, email, phone, tier
4. **Vehicle card** shows all vehicle details
5. **Status badge** displays with correct color
6. **Action dialogs** open and work correctly
7. **Back navigation** returns to correct page

---

## Next Steps After Testing

If all tests pass:
1. ? Commit your changes
2. ? Deploy to production (if ready)
3. ? Update any documentation

If tests fail:
1. Check browser console for errors
2. Check backend logs for exceptions
3. Verify database has proper data
4. Review the fix summary document
5. Re-test individual components

---

## Support

If you encounter issues:
1. Check `FIX_MANAGE_CATEGORIES_RENTALS_CUSTOMER.md` for detailed fix information
2. Review browser console errors
3. Check backend logs in the terminal
4. Verify database state with SQL queries provided above
