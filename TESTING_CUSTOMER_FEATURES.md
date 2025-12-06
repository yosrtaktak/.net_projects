# Quick Testing Guide - Customer Features

## Prerequisites
1. Backend running on `https://localhost:5000`
2. Frontend running on `https://localhost:7148`
3. Customer account registered and logged in

## Test 1: Browse and Rent a Vehicle

### Steps:
1. Navigate to `https://localhost:7148/vehicles/browse`
2. Find an available vehicle (green "Available" chip)
3. Click "Rent Now" button
4. **Expected**: Redirected to `/rentals/create?vehicleId=X`
5. **Verify**: 
   - Customer Layout is used (not Admin Layout)
   - No customer dropdown visible (auto-filled)
   - Vehicle is pre-selected
   - Start date defaults to tomorrow
   - End date defaults to 3 days from now
6. Select dates
7. **Verify**: Price calculates automatically
8. Select pricing strategy (try "weekend" or "loyalty")
9. **Verify**: Price updates with discount shown
10. Click "Book Now"
11. **Expected**: Success message, then redirect to `/my-rentals`
12. **Verify**: New rental appears in the list

### Troubleshooting:
- **404 on /api/customers/me**: Customer record doesn't exist - check database
- **Redirects to home**: Check browser console for errors
- **Price doesn't calculate**: Check customerId is loaded correctly

## Test 2: Report Damage

### Steps:
1. Navigate to `https://localhost:7148/my-rentals`
2. Find an Active or Completed rental
3. Click "Report Damage" button
4. **Expected**: Redirected to `/rentals/{rentalId}/report-damage`
5. **Verify**: 
   - Rental information displayed correctly
   - Vehicle details shown
   - Form has severity dropdown
6. Select severity: "Moderate"
7. Enter description: "Scratched left front fender in parking lot"
8. Leave cost empty (will auto-calculate)
9. Click "Submit Damage Report"
10. **Expected**: Success message, redirect to `/my-rentals`
11. Click "View Reports" on the same rental
12. **Verify**: 
    - Dialog opens
    - Damage report is listed
    - Shows "Moderate" severity
    - Shows "Reported" status
    - Shows estimated cost of $300

### Troubleshooting:
- **403 Forbidden**: Customer doesn't own the rental
- **Rental ID not found**: Rental doesn't exist
- **Submit fails**: Check description is filled

## Test 3: View Damage Reports

### Steps:
1. On `/my-rentals` page
2. Click "View Reports" on any rental with damages
3. **Expected**: Dialog opens
4. **Verify**:
   - Shows all damages for that rental
   - Color-coded severity chips
   - Status chips (Reported/Under Repair/Repaired)
   - Description and costs visible
   - Reporter name shown
   - Dates formatted correctly

## Test 4: Cancel Reservation

### Steps:
1. On `/my-rentals` page
2. Find a "Reserved" rental
3. Click "Cancel Booking"
4. **Expected**: Confirmation and status changes to "Cancelled"

## API Endpoint Tests (Using Postman or cURL)

### Get Customer Profile
```bash
curl -X GET "https://localhost:5000/api/customers/me" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```
**Expected**: Customer object with id, name, email, etc.

### Get My Rentals
```bash
curl -X GET "https://localhost:5000/api/customers/me/rentals" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```
**Expected**: Array of rental objects

### Create Rental (Customer)
```bash
curl -X POST "https://localhost:5000/api/rentals" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": 1,
    "startDate": "2024-12-10",
    "endDate": "2024-12-15",
    "pricingStrategy": "standard"
  }'
```
**Note**: customerId will be auto-filled from JWT token

### Report Damage
```bash
curl -X POST "https://localhost:5000/api/vehicledamages" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": 1,
    "rentalId": 1,
    "reportedDate": "2024-12-02",
    "description": "Small scratch on bumper",
    "severity": 0,
    "repairCost": 100
  }'
```

### Get Damages for Rental
```bash
curl -X GET "https://localhost:5000/api/vehicledamages/rental/1" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Common Issues and Solutions

### Issue: 404 on /api/customers/me
**Cause**: Customer record doesn't exist for the authenticated user

**Solution**:
```sql
-- Check if customer exists
SELECT * FROM Customers WHERE Email = 'customer@example.com';

-- If not, create one
INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber, DateOfBirth, RegistrationDate, Tier)
VALUES ('John', 'Doe', 'customer@example.com', '555-0100', 'DL123456', '1990-01-01', GETDATE(), 0);
```

### Issue: Can't see Create Rental page
**Cause**: Not logged in or token expired

**Solution**: 
1. Log out and log back in
2. Check localStorage for 'authToken'
3. Verify token is valid at jwt.io

### Issue: Vehicle not pre-selected
**Cause**: vehicleId query parameter missing

**Solution**: Make sure "Rent Now" button navigates to:
```
/rentals/create?vehicleId=1
```

### Issue: Price doesn't calculate
**Cause**: Customer ID not loaded

**Solution**: 
1. Check browser console for errors
2. Verify /api/customers/me returns data
3. Check that GetCurrentCustomerAsync() is called in OnInitializedAsync()

### Issue: Can't submit damage report
**Cause**: Validation error

**Solution**: 
1. Ensure description is not empty
2. Check that repairCost is a positive number
3. Verify rentalId is valid

## Database Queries for Debugging

### Check Customer Rentals
```sql
SELECT r.*, v.Brand, v.Model, c.Email
FROM Rentals r
JOIN Vehicles v ON r.VehicleId = v.Id
JOIN Customers c ON r.CustomerId = c.Id
WHERE c.Email = 'customer@example.com'
ORDER BY r.StartDate DESC;
```

### Check Damage Reports
```sql
SELECT vd.*, v.Brand, v.Model, r.Id as RentalId, c.Email
FROM VehicleDamages vd
JOIN Vehicles v ON vd.VehicleId = v.Id
LEFT JOIN Rentals r ON vd.RentalId = r.Id
LEFT JOIN Customers c ON r.CustomerId = c.Id
WHERE c.Email = 'customer@example.com';
```

### Check Customer Profile
```sql
SELECT c.*, COUNT(r.Id) as TotalRentals
FROM Customers c
LEFT JOIN Rentals r ON c.Id = r.CustomerId
WHERE c.Email = 'customer@example.com'
GROUP BY c.Id, c.FirstName, c.LastName, c.Email, c.PhoneNumber, 
         c.DriverLicenseNumber, c.DateOfBirth, c.Address, c.RegistrationDate, c.Tier;
```

## Success Criteria

? Customer can browse vehicles
? Customer can rent vehicles without admin help
? Customer sees their own rentals in My Rentals
? Customer can report damage during rental
? Customer can view their damage reports
? Customer can cancel reservations
? All API endpoints return correct data
? Authorization works correctly (can't access others' data)
? Price calculation works in real-time
? Navigation flows make sense

## Browser Console Checks

Open DevTools (F12) and check:

1. **Network Tab**: 
   - `/api/customers/me` returns 200
   - `/api/customers/me/rentals` returns 200
   - No 401 Unauthorized errors
   - No 403 Forbidden errors

2. **Console Tab**:
   - No JavaScript errors
   - No "Failed to fetch" errors
   - API service logs show successful calls

3. **Application Tab** (localStorage):
   - `authToken` exists
   - `username` exists
   - `userRole` is "Customer"

## Final Verification

After all tests pass:
1. Log out and log back in - everything should still work
2. Try with different browsers - Chrome, Edge, Firefox
3. Try on mobile view - responsive design should work
4. Test with multiple customers - no data leakage
5. Test concurrent rentals - no conflicts
