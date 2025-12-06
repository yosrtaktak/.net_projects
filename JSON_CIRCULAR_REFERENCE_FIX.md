# JSON Circular Reference Fix

## Problem

The application was throwing a `System.Text.Json.JsonException` due to circular references in Entity Framework navigation properties:

```
A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32.
Path: $.Vehicle.MaintenanceRecords.Vehicle.MaintenanceRecords.Vehicle...
```

### Root Cause

When Entity Framework loads entities with navigation properties, it creates circular references:
- `Vehicle` has `MaintenanceRecords`
- Each `Maintenance` has a `Vehicle` reference back to the parent
- This creates: `Vehicle ? MaintenanceRecords ? Vehicle ? MaintenanceRecords ? ...`

When ASP.NET Core tries to serialize these entities to JSON, it gets stuck in an infinite loop trying to serialize the circular references.

## Solution Applied

### 1. Configure JSON Serialization (Backend)

Updated `Backend\Program.cs` to configure JSON serialization to handle circular references:

```csharp
// Add controllers with JSON options to handle cycles
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references
        options.JsonSerializerOptions.ReferenceHandler = 
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        
        // Ignore null values to reduce payload size
        options.JsonSerializerOptions.DefaultIgnoreCondition = 
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
```

#### What This Does:

**`ReferenceHandler.IgnoreCycles`**:
- When a circular reference is detected, instead of throwing an error, it writes `null`
- Example: `Vehicle` ? `MaintenanceRecords` ? `Vehicle` (stops here, writes null instead of repeating)
- This prevents the infinite loop while still returning most of the data

**`DefaultIgnoreCondition.WhenWritingNull`**:
- Doesn't serialize properties that are `null`
- Reduces JSON payload size
- Makes responses cleaner and faster

### 2. Fixed Date Calculation in CreateRental.razor

Added proper event handlers and UI refresh:

```csharp
private async Task OnCustomerChanged(int customerId)
{
    rentalRequest.CustomerId = customerId;
    await CalculatePrice();
}

private async Task CalculatePrice()
{
    if (rentalRequest.VehicleId > 0 && rentalRequest.CustomerId > 0 &&
        startDate.HasValue && endDate.HasValue &&
        startDate < endDate)
    {
        rentalRequest.StartDate = startDate.Value;
        rentalRequest.EndDate = endDate.Value;

        var request = new CalculatePriceRequest { ... };

        try
        {
            priceCalculation = await ApiService.CalculatePriceAsync(request);
            StateHasChanged(); // ? Force UI refresh!
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error calculating price: {ex.Message}", Severity.Warning);
            priceCalculation = null;
        }
    }
    else
    {
        priceCalculation = null; // Clear when conditions not met
    }
}
```

## How to Test the Fix

### 1. **Stop the Backend** (if running)
```sh
# Press Ctrl+C in the terminal running the backend
```

### 2. **Rebuild the Backend**
```sh
cd Backend
dotnet build
```

### 3. **Restart Both Applications**

**Terminal 1 - Backend:**
```sh
cd Backend
dotnet run
```

**Terminal 2 - Frontend:**
```sh
cd Frontend
dotnet run
```

### 4. **Test the Fixed Endpoints**

#### Test Calculate Price (should no longer get 500 error):
```bash
curl -X POST "https://localhost:5000/api/rentals/calculate-price" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": 1,
    "customerId": 1,
    "startDate": "2024-12-01",
    "endDate": "2024-12-05",
    "pricingStrategy": "standard"
  }'
```

**Expected Response** (no more circular reference error):
```json
{
  "totalPrice": 200.00,
  "numberOfDays": 4,
  "dailyRate": 50.00,
  "strategyUsed": "standard",
  "discount": null
}
```

#### Test Vehicle Endpoints (should handle navigation properties correctly):
```bash
curl -X GET "https://localhost:5000/api/vehicles/1" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Expected**: Vehicle data with maintenance records, but no circular references.

#### Test Rental Creation in UI:
1. Navigate to `/rentals/create`
2. Select a customer
3. Select a vehicle
4. Choose dates
5. **Expected Results**:
   - Price summary appears immediately
   - Shows correct number of days (e.g., Dec 1 to Dec 4 = 3 days)
   - Total price calculates correctly
   - No 500 errors in console

### 5. **Verify in Browser Console**

Open browser DevTools (F12) ? Console tab:
- ? No red errors about 500 Internal Server Error
- ? No JSON serialization errors
- ? API responses complete successfully

## Alternative Solutions (Not Used)

### Option 1: Use DTOs (More Work, Better Long-term)
Create separate Data Transfer Objects without circular references:
```csharp
public class VehicleDto
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public List<MaintenanceDto> MaintenanceRecords { get; set; }
}

public class MaintenanceDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    // No Vehicle property - breaks the cycle
}
```

**Pros**: Complete control, better separation of concerns  
**Cons**: More code to write and maintain

### Option 2: Use [JsonIgnore] Attributes
Mark navigation properties to not serialize:
```csharp
public class Maintenance
{
    public int Id { get; set; }
    [JsonIgnore]
    public Vehicle Vehicle { get; set; }
}
```

**Pros**: Simple, targeted  
**Cons**: Changes domain models, might need the data sometimes

### Option 3: Use ReferenceHandler.Preserve (Not Recommended)
Adds `$id` and `$ref` metadata to JSON:
```csharp
options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
```

**Pros**: Preserves all relationships  
**Cons**: Non-standard JSON format, frontend needs special handling

## Why We Chose `IgnoreCycles`

- ? **Minimal code changes** - one configuration change fixes all endpoints
- ? **Standard JSON format** - frontend doesn't need special handling
- ? **Performance** - smaller payloads by ignoring nulls
- ? **Safety** - handles any future circular references automatically
- ? **EF Core friendly** - works with all navigation properties

## Files Modified

1. **`Backend\Program.cs`**
   - Added JSON serialization configuration
   - Lines: 99-109

2. **`Frontend\Pages\CreateRental.razor`**
   - Added `OnCustomerChanged` handler
   - Added `StateHasChanged()` in `CalculatePrice()`
   - Added null check for price calculation

## Verification Checklist

After restarting both applications:

- [ ] Backend starts without errors
- [ ] Frontend starts without errors
- [ ] Can access `/rentals/create` page
- [ ] Selecting customer triggers price calculation
- [ ] Selecting vehicle updates price
- [ ] Changing dates shows correct number of days
- [ ] No 500 errors in browser console
- [ ] No JSON cycle errors in backend logs
- [ ] Can create rental successfully
- [ ] Can access maintenance endpoints without errors
- [ ] Can access vehicle damage endpoints without errors

## Next Steps

If you still see issues:

1. **Clear browser cache**: Hard refresh (Ctrl+Shift+R)
2. **Check backend logs**: Look for any remaining serialization errors
3. **Verify token**: Make sure JWT token is valid
4. **Check database**: Ensure data exists for testing

## Long-term Recommendations

1. **Consider implementing DTOs** for complex endpoints with deep object graphs
2. **Add AutoMapper** to handle entity-to-DTO mapping automatically
3. **Implement lazy loading carefully** to avoid loading unnecessary navigation properties
4. **Use `.AsNoTracking()`** for read-only queries to improve performance

## Benefits of This Fix

- ?? **All API endpoints work correctly** - no more 500 errors
- ? **Smaller JSON payloads** - ignoring nulls reduces bandwidth
- ?? **Handles future circular references** - new relationships won't break
- ?? **Standard JSON** - frontend code unchanged
- ??? **Safe and robust** - prevents serialization crashes

The fix is now complete! Restart your backend and the circular reference errors should be resolved.
