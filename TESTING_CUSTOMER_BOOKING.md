# Testing Guide: Customer Self-Service Booking

## Quick Start Testing

### Prerequisites
1. ? Backend running: `https://localhost:5000`
2. ? Frontend running: `https://localhost:7148`
3. ? Customer account created and logged in
4. ? Customer record exists in database (run `create_missing_customer_records.sql` if needed)

## Test Scenario 1: Customer Books a Vehicle

### Step-by-Step Test

1. **Login as Customer**
   - Navigate to `https://localhost:7148/login`
   - Login with customer credentials
   - Should see Customer layout with navigation

2. **Browse Vehicles**
   - Click "Browse Vehicles" in navigation or go to `/vehicles/browse`
   - **Expected**: See grid of available vehicles
   - **Verify**: Only vehicles with "Available" status shown
   - **Verify**: Each vehicle card has a green "Available" chip
   - **Verify**: "Rent Now" button visible on available vehicles

3. **Click Rent Now**
   - Click "Rent Now" on any available vehicle (e.g., Tesla Model 3)
   - **Expected**: Redirected to `/book-vehicle/1` (vehicle ID in URL)
   - **Verify**: Vehicle is pre-selected in dropdown
   - **Verify**: Vehicle details card shows make, model, category, year, fuel, seats
   - **Verify**: Customer info card shows your name, email, tier
   - **Verify**: Default dates set (tomorrow and 3 days later)

4. **Configure Booking**
   - **Pick-up Date**: Select tomorrow's date
   - **Return Date**: Select date 5 days from now
   - **Expected**: "Duration: 5 days" alert appears
   - **Verify**: Return date picker minimum is 1 day after pick-up
   - **Verify**: Price summary appears automatically

5. **Select Pricing Strategy**
   - Try each option:
     - ? Standard Rate (default)
     - ? Weekend Special
     - ? Seasonal Rate
     - ? Loyalty Discount
   - **Expected**: Price updates for each selection
   - **Verify**: Discount percentage shown for non-standard rates
   - **Verify**: Total price updates in real-time

6. **Review Price Summary**
   - Check price summary card shows:
     - ? Rental Duration (X days)
     - ? Daily Rate ($XX.XX)
     - ? Pricing Plan (selected strategy)
     - ? Discount (if applicable, e.g., -10%)
     - ? Total Cost ($XXX.XX) in large text
   - **Verify**: "Confirm Booking" button shows total price

7. **Submit Booking**
   - Click "Confirm Booking - $XXX.XX" button
   - **Expected**: 
     - Button shows "Processing Booking..." with spinner
     - Success message appears: "Booking confirmed! Total: $XXX.XX"
     - After 2 seconds, redirect to `/my-rentals`
   - **Verify**: New rental appears in My Rentals list
   - **Verify**: Rental status is "Reserved"
   - **Verify**: Correct dates, vehicle, and price shown

## Test Scenario 2: Customer Cancels Booking

1. **Navigate to My Rentals**
   - Go to `/my-rentals`
   - **Verify**: See list of your rentals
   - **Verify**: "Reserved" rentals have "Cancel Booking" button

2. **Cancel a Reserved Rental**
   - Find a rental with status "Reserved"
   - Click "Cancel Booking" button
   - **Expected**: Confirmation dialog appears
   - Confirm cancellation
   - **Expected**: Success message
   - **Verify**: Rental status changes to "Cancelled"
   - **Verify**: Vehicle becomes available again

3. **Try to Cancel Active Rental**
   - Find a rental with status "Active" (if you have one)
   - **Expected**: No "Cancel Booking" button visible
   - **Reason**: Customers can only cancel Reserved rentals

## Test Scenario 3: Admin Creates Rental

1. **Login as Admin/Employee**
   - Logout from customer account
   - Login with admin credentials

2. **Navigate to Create Rental**
   - Go to `/rentals/manage`
   - Click "Create Rental" button
   - **Expected**: Redirected to `/rentals/create`
   - **Verify**: Admin Layout is used
   - **Verify**: Customer dropdown is visible and required

3. **Create Rental for Customer**
   - Select customer from dropdown
   - Select vehicle
   - Select dates
   - Choose pricing strategy
   - **Verify**: Price calculates correctly
   - Click "Create Rental"
   - **Expected**: Success message and redirect to `/rentals/manage`

## Test Scenario 4: Security Testing

### Test: Customer Cannot Access Admin Page
1. Login as Customer
2. Try to navigate to `/rentals/create`
3. **Expected**: Redirected to home page or access denied
4. **Verify**: Cannot see admin rental creation page

### Test: Customer Cannot Book for Others
1. Login as Customer
2. Open browser DevTools (F12)
3. Go to `/book-vehicle`
4. Modify the API request to include a different customer ID
5. Submit booking
6. **Expected**: Backend overrides customer ID with your own
7. **Verify**: Rental created for YOU, not the other customer

### Test: Admin Can Create for Any Customer
1. Login as Admin
2. Go to `/rentals/create`
3. Select any customer from dropdown
4. Create rental
5. **Expected**: Rental created for selected customer
6. **Verify**: Rental appears in that customer's rentals

## Test Scenario 5: Error Handling

### Test: Vehicle Not Available
1. Customer attempts to book vehicle already rented for those dates
2. **Expected**: Error message: "Vehicle is not available for the selected dates"
3. **Verify**: Booking fails gracefully

### Test: Invalid Date Range
1. Select return date before pick-up date
2. **Expected**: Button disabled, cannot submit
3. **Verify**: Return date picker enforces minimum date

### Test: No Customer Record
1. Login with user account that has no Customer record
2. Try to access `/book-vehicle`
3. **Expected**: Error alert: "Unable to load your customer profile"
4. **Solution**: Run `create_missing_customer_records.sql`

## Test Scenario 6: Navigation Flow

### Test: Complete Customer Journey
1. Start at `/` (home page)
2. Click "Browse Vehicles"
3. Filter by category (e.g., SUV)
4. Click "Rent Now" on a vehicle
5. Configure booking and submit
6. Navigate to "My Rentals"
7. View rental details
8. Cancel booking (if Reserved)
9. Return to "Browse Vehicles"
10. **Verify**: Cancelled vehicle is available again

## Verification Checklist

### Customer Booking Page (/book-vehicle)
- [ ] Page loads without errors
- [ ] Customer info displays correctly
- [ ] Vehicle pre-selected when coming from browse
- [ ] Vehicle dropdown shows all available vehicles
- [ ] Date pickers work correctly
- [ ] Price calculates in real-time
- [ ] All pricing strategies work
- [ ] Discount displayed for special rates
- [ ] Confirm button disabled when invalid
- [ ] Success message on successful booking
- [ ] Redirect to My Rentals after booking
- [ ] Mobile responsive design

### Admin Rental Creation (/rentals/create)
- [ ] Page restricted to Admin/Employee
- [ ] Customer dropdown visible and required
- [ ] Vehicle selection works
- [ ] Date selection works
- [ ] Price calculation works
- [ ] Create button disabled when invalid
- [ ] Success message on creation
- [ ] Redirect to Manage Rentals after creation

### Security
- [ ] Customer cannot access /rentals/create
- [ ] Customer ID automatically set from token
- [ ] Customer cannot book for others
- [ ] Customer can only cancel their own Reserved rentals
- [ ] Admin can create for any customer
- [ ] JWT token required for all operations

### API Endpoints
- [ ] `POST /api/rentals` works for customers
- [ ] `POST /api/rentals` works for admin
- [ ] `GET /api/customers/me` returns current customer
- [ ] `PUT /api/rentals/{id}/cancel` works for customers
- [ ] `POST /api/rentals/calculate-price` returns correct price

## Common Issues and Solutions

### Issue: "Unable to load customer profile"
**Cause**: No Customer record for user
**Solution**: 
```sql
-- Run this SQL script
USE CarRentalDB;
EXEC sp_executesql N'
INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber, DateOfBirth, RegistrationDate, Tier)
SELECT u.UserName, '''', u.Email, '''', '''', DATEADD(YEAR, -25, GETDATE()), u.CreatedAt, 0
FROM AspNetUsers u
LEFT JOIN Customers c ON u.Email = c.Email
WHERE c.Id IS NULL AND u.Email = @Email',
N'@Email nvarchar(256)',
@Email = N'customer@example.com';
```

### Issue: Price doesn't calculate
**Symptoms**: Price summary shows "Select vehicle and dates..."
**Causes & Solutions**:
1. Customer profile not loaded ? Check `/api/customers/me` endpoint
2. Invalid date range ? Ensure return date is after pick-up date
3. No vehicle selected ? Select a vehicle from dropdown
4. Backend error ? Check browser console and backend logs

### Issue: Redirect to home after booking
**Cause**: Authorization failure
**Solution**: 
1. Check JWT token in localStorage
2. Verify token hasn't expired
3. Logout and login again
4. Check user has "Customer" role

### Issue: "Vehicle is not available"
**Cause**: Vehicle already booked for selected dates
**Solution**:
1. Try different dates
2. Select a different vehicle
3. Check in database:
```sql
SELECT * FROM Rentals 
WHERE VehicleId = 1 
AND Status IN ('Reserved', 'Active')
AND (
    (StartDate <= '2024-12-15' AND EndDate >= '2024-12-10')
);
```

## Database Verification Queries

### Check Customer Rentals
```sql
SELECT r.Id, r.Status, r.StartDate, r.EndDate, r.TotalCost,
       v.Brand, v.Model, c.Email
FROM Rentals r
JOIN Vehicles v ON r.VehicleId = v.Id
JOIN Customers c ON r.CustomerId = c.Id
WHERE c.Email = 'customer@example.com'
ORDER BY r.CreatedAt DESC;
```

### Check Vehicle Availability
```sql
SELECT v.Id, v.Brand, v.Model, v.Status,
       COUNT(r.Id) as ActiveRentals
FROM Vehicles v
LEFT JOIN Rentals r ON v.Id = r.VehicleId 
    AND r.Status IN ('Reserved', 'Active')
GROUP BY v.Id, v.Brand, v.Model, v.Status;
```

### Check Customer Profile
```sql
SELECT c.*, u.Email as UserEmail
FROM Customers c
JOIN AspNetUsers u ON c.Email = u.Email
WHERE c.Email = 'customer@example.com';
```

## Performance Testing

### Test: Concurrent Bookings
1. Open 2 browser windows
2. Login as different customers
3. Try to book same vehicle for same dates simultaneously
4. **Expected**: One succeeds, one gets "not available" error
5. **Verify**: No double-booking in database

### Test: Price Calculation Speed
1. Select vehicle and dates
2. Change pricing strategy multiple times quickly
3. **Expected**: Price updates within 1 second each time
4. **Verify**: No flickering or multiple updates

## Success Criteria

All tests should pass with:
- ? No console errors
- ? Correct navigation flow
- ? Proper authorization enforcement
- ? Accurate price calculations
- ? Data persisted correctly in database
- ? User-friendly error messages
- ? Responsive design on mobile
- ? Fast page loads (<2 seconds)

## Automated Testing (Optional)

### Example Test Cases
```csharp
[Fact]
public async Task Customer_CanBookVehicle()
{
    // Arrange
    var customer = await CreateCustomer();
    var vehicle = await CreateAvailableVehicle();
    
    // Act
    var rental = await BookVehicle(customer, vehicle, DateTime.Today.AddDays(1), DateTime.Today.AddDays(5));
    
    // Assert
    Assert.NotNull(rental);
    Assert.Equal(customer.Id, rental.CustomerId);
    Assert.Equal(vehicle.Id, rental.VehicleId);
    Assert.Equal(RentalStatus.Reserved, rental.Status);
}

[Fact]
public async Task Customer_CannotBookForOtherCustomer()
{
    // Arrange
    var customer1 = await CreateCustomer("customer1@test.com");
    var customer2 = await CreateCustomer("customer2@test.com");
    var vehicle = await CreateAvailableVehicle();
    
    // Act
    var rental = await BookVehicleAs(customer1, customer2.Id, vehicle, DateTime.Today.AddDays(1), DateTime.Today.AddDays(5));
    
    // Assert
    Assert.Equal(customer1.Id, rental.CustomerId); // Backend overrides
}
```

## Final Checklist

Before marking as complete:
- [ ] All manual tests pass
- [ ] No errors in browser console
- [ ] No errors in backend logs
- [ ] Database has correct data
- [ ] Security tests pass
- [ ] Navigation flows work
- [ ] Error handling works
- [ ] Mobile responsive
- [ ] Performance acceptable
- [ ] Documentation updated

## Report Issues

If you encounter issues:
1. Check browser console (F12)
2. Check backend logs
3. Verify database state
4. Check JWT token validity
5. Try logout/login
6. Clear browser cache
7. Restart backend if needed

Document any issues with:
- Steps to reproduce
- Expected vs actual behavior
- Error messages
- Screenshots
- Console logs
