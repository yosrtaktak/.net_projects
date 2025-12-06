# Customer Self-Service Rental Implementation

## Overview
The car rental system now has **separate rental flows** for Customers vs. Admin/Employee roles:

- **Customers**: Use the streamlined `/book-vehicle` page
- **Admin/Employee**: Use the full-featured `/rentals/create` page

## Architecture

### Customer Rental Flow

```
Browse Vehicles (/vehicles/browse)
    ? [Click "Rent Now"]
Book Vehicle (/book-vehicle/{vehicleId})
    ? [Select dates & pricing, confirm]
My Rentals (/my-rentals)
    ? [View booking details, cancel if needed]
```

### Admin/Employee Rental Flow

```
Manage Rentals (/rentals/manage)
    ? [Click "Create Rental"]
Create Rental (/rentals/create)
    ? [Select customer, vehicle, dates]
Manage Rentals (/rentals/manage)
    ? [Complete, cancel, or update rentals]
```

## Key Features

### 1. Customer Booking Page (`/book-vehicle`)
**Route**: `/book-vehicle` or `/book-vehicle/{vehicleId}`
**Authorization**: Customer role only
**Layout**: CustomerLayout

**Features**:
- ? Pre-selected vehicle when coming from Browse page
- ? Automatic customer identification (no dropdown needed)
- ? Real-time price calculation
- ? Visual pricing strategy selection with descriptions
- ? Customer info display (name, email, tier)
- ? Beautiful gradient UI optimized for customer experience
- ? Duration calculator
- ? Vehicle details preview card
- ? Redirect to My Rentals after successful booking

**User Experience**:
1. Customer browses vehicles
2. Clicks "Rent Now" on desired vehicle
3. Automatically navigated to booking page with vehicle pre-selected
4. Selects pick-up and return dates
5. Chooses pricing strategy (Standard, Weekend, Seasonal, Loyalty)
6. Views real-time price calculation
7. Confirms booking with one click
8. Redirected to see their active rentals

### 2. Admin/Employee Rental Creation (`/rentals/create`)
**Route**: `/rentals/create`
**Authorization**: Admin, Employee roles only
**Layout**: AdminLayout

**Features**:
- ? Customer selection dropdown (required)
- ? Vehicle selection dropdown
- ? Date selection
- ? Pricing strategy selection
- ? Real-time price calculation
- ? Redirect to Manage Rentals after creation

**User Experience**:
1. Admin/Employee navigates to Manage Rentals
2. Clicks "Create Rental"
3. Selects which customer to book for
4. Selects vehicle and dates
5. Chooses pricing strategy
6. Views price calculation
7. Creates rental on behalf of customer

### 3. Backend Security
**File**: `Backend/Controllers/RentalsController.cs`

**Customer Protection**:
```csharp
[HttpPost]
public async Task<ActionResult<Rental>> CreateRental([FromBody] CreateRentalDto dto)
{
    // If customer role, automatically use their customer ID
    if (User.IsInRole("Customer"))
    {
        var userEmail = User.Identity?.Name;
        var customer = customers.FirstOrDefault(c => c.Email == userEmail);
        dto.CustomerId = customer.Id; // Override any provided customerId
    }
    // ... create rental
}
```

**Benefits**:
- Customers cannot create rentals for other customers
- Customer ID is taken from JWT token, not request body
- Secure even if front-end is compromised

### 4. Customer Rental Cancellation
**Endpoint**: `PUT /api/rentals/{id}/cancel`
**Authorization**: Admin, Employee, or owning Customer

**Customer Rules**:
- ? Can cancel their own rentals
- ? Can only cancel "Reserved" status rentals
- ? Cannot cancel Active or Completed rentals
- ? Cannot cancel other customers' rentals

**Code**:
```csharp
[HttpPut("{id}/cancel")]
public async Task<ActionResult<Rental>> CancelRental(int id)
{
    if (User.IsInRole("Customer"))
    {
        var rental = await _rentalService.GetRentalByIdAsync(id);
        
        // Verify ownership
        if (rental.Customer?.Email != User.Identity?.Name)
            return Forbid();
        
        // Only Reserved rentals can be cancelled by customers
        if (rental.Status != RentalStatus.Reserved)
            return BadRequest("Only reserved rentals can be cancelled");
    }
    
    var updatedRental = await _rentalService.CancelRentalAsync(id);
    return Ok(updatedRental);
}
```

## Files Modified

### Frontend Pages
1. **NEW**: `Frontend/Pages/BookVehicle.razor` - Customer booking page
2. **MODIFIED**: `Frontend/Pages/BrowseVehicles.razor` - Navigate to `/book-vehicle/{id}`
3. **MODIFIED**: `Frontend/Pages/CreateRental.razor` - Admin/Employee only, AdminLayout

### Backend Controllers
1. `Backend/Controllers/RentalsController.cs` - Already had customer protection

### Frontend Services
- `Frontend/Services/ApiService.cs` - All endpoints already available

## API Endpoints

### Customer Endpoints
```
POST   /api/rentals                    - Create rental (customer auto-identified)
GET    /api/customers/me                - Get current customer profile
GET    /api/customers/me/rentals        - Get my rentals
PUT    /api/rentals/{id}/cancel         - Cancel own rental (Reserved only)
POST   /api/rentals/calculate-price     - Calculate rental price
```

### Admin/Employee Endpoints
```
POST   /api/rentals                    - Create rental for any customer
GET    /api/rentals                     - Get all rentals
GET    /api/rentals/manage              - Get rentals with filters
PUT    /api/rentals/{id}/complete       - Complete rental
PUT    /api/rentals/{id}/cancel         - Cancel any rental
PUT    /api/rentals/{id}/status         - Update rental status
```

## User Workflows

### Customer Books a Vehicle
1. **Browse**: Customer visits `/vehicles/browse`
2. **Select**: Clicks "Rent Now" on Tesla Model 3
3. **Book**: Navigated to `/book-vehicle/1` (vehicle pre-selected)
4. **Configure**: 
   - Pick-up: Tomorrow
   - Return: 5 days later
   - Pricing: Loyalty Discount
5. **Review**: Sees total price: $450 (10% discount applied)
6. **Confirm**: Clicks "Confirm Booking - $450"
7. **Success**: Redirected to `/my-rentals`, sees new booking

### Customer Cancels Booking
1. **View**: Customer visits `/my-rentals`
2. **Find**: Sees a Reserved rental for next week
3. **Cancel**: Clicks "Cancel Booking" button
4. **Confirm**: Confirms cancellation
5. **Result**: Rental status changes to Cancelled, vehicle becomes available

### Admin Creates Rental for Customer
1. **Manage**: Admin visits `/rentals/manage`
2. **Create**: Clicks "Create Rental"
3. **Select Customer**: Chooses "John Doe" from dropdown
4. **Select Vehicle**: Chooses BMW X5
5. **Set Dates**: Tomorrow to next Friday
6. **Price**: Reviews calculated price
7. **Submit**: Clicks "Create Rental"
8. **Result**: Rental created, admin returns to management view

## Security Features

### Authorization Layers
1. **Route Authorization**: `@attribute [Authorize(Roles = "Customer")]`
2. **Backend Validation**: Customer ID override in controller
3. **JWT Token**: Email verification from token
4. **Ownership Checks**: Verify rental belongs to customer

### Prevented Attacks
- ? Customer cannot book for other customers (backend override)
- ? Customer cannot cancel others' rentals (ownership check)
- ? Customer cannot access admin rental creation (route authorization)
- ? Customer cannot complete rentals (endpoint authorization)
- ? Tampering with customer ID in request (backend override)

## UI/UX Improvements

### Customer Booking Page
- **Gradient Header**: Purple gradient for premium feel
- **Vehicle Preview**: Shows selected vehicle details
- **Real-time Pricing**: Updates as user changes options
- **Clear CTAs**: "Confirm Booking - $X.XX" button shows total
- **Visual Feedback**: Loading states, success messages
- **Mobile Responsive**: Works on all screen sizes

### Admin Creation Page
- **Professional UI**: Standard admin theme
- **Full Control**: Can select any customer
- **Bulk Operations**: Can create multiple rentals quickly
- **Management Tools**: Direct access to rental management

## Testing

### Test Customer Booking
```bash
# 1. Register as customer
curl -X POST "https://localhost:5000/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "customer1",
    "email": "customer1@test.com",
    "password": "Test@123",
    "role": "Customer"
  }'

# 2. Login
curl -X POST "https://localhost:5000/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "customer1@test.com",
    "password": "Test@123"
  }'

# 3. Create rental (customer ID auto-filled)
curl -X POST "https://localhost:5000/api/rentals" \
  -H "Authorization: Bearer {TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": 1,
    "startDate": "2024-12-10",
    "endDate": "2024-12-15",
    "pricingStrategy": "loyalty"
  }'
```

### Test Security
```bash
# Try to create rental with wrong customer ID (should be overridden)
curl -X POST "https://localhost:5000/api/rentals" \
  -H "Authorization: Bearer {CUSTOMER_TOKEN}" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": 999,
    "vehicleId": 1,
    "startDate": "2024-12-10",
    "endDate": "2024-12-15",
    "pricingStrategy": "standard"
  }'

# Backend will use correct customer ID from token, ignoring 999
```

## Database Considerations

### Customer Record Required
Before a user can book vehicles, they must have a Customer record:

```sql
-- Check if customer exists
SELECT * FROM Customers WHERE Email = 'user@example.com';

-- Create if missing (run create_missing_customer_records.sql)
INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber, DriverLicenseNumber, DateOfBirth, RegistrationDate, Tier)
SELECT 
    u.UserName, '', u.Email, '', '', 
    DATEADD(YEAR, -25, GETDATE()), 
    u.CreatedAt, 0
FROM AspNetUsers u
WHERE u.Email = 'user@example.com'
AND NOT EXISTS (SELECT 1 FROM Customers WHERE Email = u.Email);
```

## Configuration

### Route Configuration
No additional configuration needed. Routes are defined with `@page` directive.

### Authorization
Uses existing JWT authentication. No changes to `Program.cs` required.

### Navigation
Update `CustomerLayout.razor` if you want to add a direct "Book Now" link to navigation.

## Future Enhancements

### Potential Additions
1. **Date Availability Calendar**: Visual calendar showing available dates
2. **Vehicle Comparison**: Compare multiple vehicles side-by-side
3. **Booking History**: View past bookings with reviews
4. **Favorite Vehicles**: Save preferred vehicles for quick booking
5. **Recurring Bookings**: Set up weekly/monthly vehicle reservations
6. **Price Alerts**: Notify when prices drop for saved vehicles
7. **Booking Modifications**: Change dates without cancelling
8. **Early Return**: Process early returns with refunds

### Technical Improvements
1. **Optimistic Locking**: Prevent double-booking race conditions
2. **Payment Integration**: Stripe/PayPal integration
3. **Email Notifications**: Booking confirmations via email
4. **SMS Reminders**: Send pickup reminders
5. **QR Codes**: Generate QR codes for vehicle pickup
6. **Digital Contracts**: E-signature rental agreements

## Troubleshooting

### Issue: Customer can't access booking page
**Solution**: Verify user has "Customer" role in JWT token

### Issue: 404 on /api/customers/me
**Solution**: Run `create_missing_customer_records.sql` to create Customer record

### Issue: Price doesn't calculate
**Solution**: Check that customer profile loads successfully

### Issue: Booking redirects to home
**Solution**: Check browser console for authorization errors

### Issue: Customer ID wrong in database
**Solution**: Backend automatically overrides - check RentalsController logic

## Success Metrics

? **Separation of Concerns**: Different UIs for different roles
? **Security**: Customers can only book for themselves
? **User Experience**: Streamlined booking process for customers
? **Flexibility**: Admins retain full control
? **Scalability**: Easy to add new features to either flow
? **Maintainability**: Clear code separation between roles

## Summary

The rental system now provides:
- **Customer-focused booking experience** through `/book-vehicle`
- **Admin-focused rental management** through `/rentals/create`
- **Secure authorization** preventing unauthorized bookings
- **Seamless navigation** from browse to book to manage
- **Real-time pricing** with multiple strategies
- **Self-service cancellation** for customers

This implementation follows best practices for multi-role applications while maintaining security and providing an excellent user experience for all roles.
