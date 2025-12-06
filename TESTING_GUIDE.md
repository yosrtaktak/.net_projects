# Testing Guide - Reports and Rentals Management

## Prerequisites
1. Backend API running on `https://localhost:5000`
2. Frontend running on `https://localhost:7148`
3. Login as Admin or Employee user

## Test Scenarios

### 1. Test Reports Dashboard

#### Access the Reports Page
```
URL: https://localhost:7148/reports
Required Role: Admin or Employee
```

#### Expected Data
- **Key Metrics**: Should show 4 cards with revenue and vehicle counts
- **Rental Status**: Breakdown of Active, Reserved, Completed, Cancelled
- **Category Table**: Vehicles grouped by category with availability rates
- **Top Vehicles**: Top 5 most rented vehicles with revenue
- **Revenue Trend**: Monthly revenue for last 6 months
- **Recent Rentals**: Last 10 rental transactions

#### Test Actions
1. Click "Refresh Data" button - should reload all report data
2. Verify all numbers are accurate and match database
3. Check that charts/tables display correctly

### 2. Test Rentals Management

#### Access the Manage Rentals Page
```
URL: https://localhost:7148/rentals/manage
Required Role: Admin or Employee
```

#### Test Filtering
1. **Filter by Status**:
   - Select "Active" from Status dropdown
   - Click "Apply Filters"
   - Verify only Active rentals are shown

2. **Filter by Date Range**:
   - Select Start Date
   - Select End Date
   - Click "Apply Filters"
   - Verify rentals within date range

3. **Clear Filters**:
   - Click "Clear" button
   - Verify all rentals are shown again

#### Test Rental Actions

##### Complete Rental
1. Find a rental with status "Active" or "Reserved"
2. Click the three-dot menu (?)
3. Select "Complete"
4. Enter End Mileage in dialog
5. Click "Complete"
6. Verify:
   - Success message appears
   - Rental status changes to "Completed"
   - Vehicle status changes to "Available"
   - Table refreshes

##### Cancel Rental
1. Find a rental with status "Active" or "Reserved"
2. Click the three-dot menu (?)
3. Select "Cancel"
4. Verify:
   - Success message appears
   - Rental status changes to "Cancelled"
   - Vehicle status changes to "Available"
   - Table refreshes

##### Update Status
1. Click the three-dot menu (?) on any rental
2. Select "Update Status"
3. Choose new status from dropdown
4. Click "Update"
5. Verify:
   - Success message appears
   - Status updates in table
   - Vehicle status syncs appropriately

##### View Details
1. Click the three-dot menu (?)
2. Select "View Details"
3. Should navigate to rental details page

### 3. Test API Endpoints Directly

#### Reports Endpoints
```bash
# Get Dashboard Report
curl -X GET "https://localhost:5000/api/reports/dashboard" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Get Rental Statistics
curl -X GET "https://localhost:5000/api/reports/rentals/statistics?startDate=2024-01-01&endDate=2024-12-31" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Get Vehicle Utilization
curl -X GET "https://localhost:5000/api/reports/vehicles/utilization?startDate=2024-01-01&endDate=2024-12-31" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Get Monthly Revenue
curl -X GET "https://localhost:5000/api/reports/revenue/monthly?months=12" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### Rentals Management Endpoints
```bash
# Get Rentals with Filters
curl -X GET "https://localhost:5000/api/rentals/manage?status=Active" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# Update Rental Status
curl -X PUT "https://localhost:5000/api/rentals/1/status" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"status": "Active"}'

# Complete Rental
curl -X PUT "https://localhost:5000/api/rentals/1/complete" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"endMileage": 50000}'

# Cancel Rental
curl -X PUT "https://localhost:5000/api/rentals/1/cancel" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Expected Behaviors

### Reports Page
- ? Shows real-time data from database
- ? Updates when "Refresh Data" is clicked
- ? Displays charts and tables correctly
- ? Access denied for non-admin/employee users
- ? Responsive design works on all screen sizes

### Manage Rentals Page
- ? Lists all rentals by default
- ? Filters work correctly
- ? Statistics cards update with filtered data
- ? Complete dialog validates mileage input
- ? Status updates sync vehicle status
- ? Success/error messages display appropriately
- ? Table refreshes after actions

## Common Issues & Solutions

### Issue: "Access Denied" Message
**Solution**: Ensure you're logged in as Admin or Employee

### Issue: No Data Showing in Reports
**Solution**: 
1. Check that rentals exist in database
2. Verify API is running
3. Check browser console for errors
4. Ensure JWT token is valid

### Issue: Filters Not Working
**Solution**:
1. Check date format (should be yyyy-MM-dd)
2. Verify status values match enum
3. Check browser console for API errors

### Issue: Complete Rental Fails
**Solution**:
1. Ensure end mileage is greater than start mileage
2. Verify rental status is Active or Reserved
3. Check that vehicle exists

### Issue: Status Update Not Syncing Vehicle
**Solution**:
1. Check RentalService.UpdateRentalStatusAsync logic
2. Verify vehicle relationship is loaded
3. Check database constraints

## Database Verification Queries

```sql
-- Check rental counts by status
SELECT Status, COUNT(*) as Count
FROM Rentals
GROUP BY Status;

-- Check total revenue
SELECT SUM(TotalCost) as TotalRevenue
FROM Rentals
WHERE Status = 4; -- Completed

-- Check vehicle utilization
SELECT v.Brand, v.Model, COUNT(r.Id) as RentalCount
FROM Vehicles v
LEFT JOIN Rentals r ON v.Id = r.VehicleId
WHERE r.Status = 4
GROUP BY v.Id, v.Brand, v.Model
ORDER BY RentalCount DESC;

-- Check monthly revenue
SELECT 
    YEAR(StartDate) as Year,
    MONTH(StartDate) as Month,
    SUM(TotalCost) as Revenue,
    COUNT(*) as RentalCount
FROM Rentals
WHERE Status = 4
GROUP BY YEAR(StartDate), MONTH(StartDate)
ORDER BY Year DESC, Month DESC;
```

## Performance Considerations

1. **Large Datasets**: Reports may take longer with thousands of rentals
2. **Caching**: Consider implementing caching for dashboard reports
3. **Pagination**: Add pagination to Manage Rentals table for large datasets
4. **Indexes**: Ensure database indexes on Status, StartDate, EndDate columns

## Next Steps

1. Test all scenarios listed above
2. Verify data accuracy against database
3. Check authorization for different user roles
4. Test responsive design on mobile devices
5. Monitor API performance with browser dev tools
