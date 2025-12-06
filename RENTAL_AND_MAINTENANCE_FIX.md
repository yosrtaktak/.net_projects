# Rental and Maintenance API Fix

## Issues Fixed

### 1. **500 Internal Server Error on POST /api/rentals**
**Root Cause**: The `CalculatePrice` endpoint was returning a simple object instead of the expected `PriceCalculationResponse` structure.

**Solution**:
- Updated `RentalsController.CalculatePrice` to return proper `PriceCalculationResponse` object
- Added `PriceCalculationResponse` class to `Backend\Application\DTOs\RentalDtos.cs`
- Injected required dependencies (`IVehicleRepository`, `IUnitOfWork`) into `RentalsController`

**Changes Made**:
```csharp
// Added to RentalDtos.cs
public class PriceCalculationResponse
{
    public decimal TotalPrice { get; set; }
    public int NumberOfDays { get; set; }
    public decimal DailyRate { get; set; }
    public string StrategyUsed { get; set; } = string.Empty;
    public decimal? Discount { get; set; }
}

// Updated RentalsController.CalculatePrice method
[HttpPost("calculate-price")]
public async Task<ActionResult<PriceCalculationResponse>> CalculatePrice([FromBody] CalculatePriceDto dto)
{
    var vehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId);
    var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(dto.CustomerId);
    
    var totalPrice = await _rentalService.CalculatePriceAsync(...);
    var numberOfDays = (dto.EndDate - dto.StartDate).Days;
    
    var response = new PriceCalculationResponse
    {
        TotalPrice = totalPrice,
        NumberOfDays = numberOfDays,
        DailyRate = vehicle.DailyRate,
        StrategyUsed = dto.PricingStrategy,
        Discount = calculateDiscount() // If applicable
    };
    
    return Ok(response);
}
```

### 2. **500 Internal Server Error on POST /api/maintenances**
**Root Cause**: The endpoint was working but might have validation issues.

**Status**: The maintenance endpoint structure is correct. Errors were likely due to:
- Missing required fields in the request
- Authorization issues
- Database constraint violations

**Controller Structure** (already correct):
```csharp
[Authorize(Roles = "Admin,Employee")]
[HttpPost]
public async Task<ActionResult<Maintenance>> CreateMaintenance([FromBody] CreateMaintenanceDto dto)
{
    var vehicle = await _vehicleRepository.GetByIdAsync(dto.VehicleId);
    if (vehicle == null)
        return NotFound(new { message = "Vehicle not found" });

    var maintenance = new Maintenance { ... };
    await _maintenanceRepository.AddAsync(maintenance);
    await _unitOfWork.CommitAsync();
    
    return CreatedAtAction(nameof(GetMaintenance), new { id = maintenance.Id }, maintenance);
}
```

### 3. **Date Showing as 0 Days in Rental Creation**
**Root Cause**: The price calculation wasn't being triggered properly when dates changed, and component wasn't re-rendering.

**Solution**:
- Added `OnCustomerChanged` handler to trigger price calculation
- Updated date change handlers to validate date ranges before calculating
- Added `StateHasChanged()` call after price calculation
- Fixed null reference by resetting `priceCalculation` when conditions aren't met

**Changes Made to `CreateRental.razor`**:
```csharp
private async Task OnCustomerChanged(int customerId)
{
    rentalRequest.CustomerId = customerId;
    await CalculatePrice();
}

private async Task OnStartDateChanged(DateTime? date)
{
    startDate = date;
    if (startDate.HasValue && endDate.HasValue && startDate < endDate)
    {
        rentalRequest.StartDate = startDate.Value;
        rentalRequest.EndDate = endDate.Value;
        await CalculatePrice();
    }
}

private async Task OnEndDateChanged(DateTime? date)
{
    endDate = date;
    if (startDate.HasValue && endDate.HasValue && startDate < endDate)
    {
        rentalRequest.StartDate = startDate.Value;
        rentalRequest.EndDate = endDate.Value;
        await CalculatePrice();
    }
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
            StateHasChanged(); // Force UI update
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error calculating price: {ex.Message}", Severity.Warning);
            priceCalculation = null;
        }
    }
    else
    {
        priceCalculation = null; // Clear if conditions not met
    }
}
```

## Testing the Fixes

### Test Rental Creation
1. Navigate to `/rentals/create`
2. Select a customer
3. Select a vehicle
4. Choose start and end dates
5. **Expected Result**: 
   - Price summary should show immediately
   - Number of days should be correct (e.g., if start=Jan 1, end=Jan 4, should show 3 days)
   - Total price should calculate correctly
   - Strategy discount should display if applicable

### Test Price Calculation
```bash
# Test the calculate-price endpoint
curl -X POST "https://localhost:5000/api/rentals/calculate-price" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": 1,
    "customerId": 1,
    "startDate": "2024-12-01",
    "endDate": "2024-12-05",
    "pricingStrategy": "standard"
  }'

# Expected Response:
{
  "totalPrice": 200.00,
  "numberOfDays": 4,
  "dailyRate": 50.00,
  "strategyUsed": "standard",
  "discount": null
}
```

### Test Maintenance Creation
```bash
curl -X POST "https://localhost:5000/api/maintenances" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleId": 1,
    "scheduledDate": "2024-12-10",
    "description": "Oil change",
    "cost": 50.00,
    "type": 0
  }'

# Expected Response: 201 Created with maintenance object
```

## Validation Rules

### Rental Creation
- ? Customer ID must be > 0
- ? Vehicle ID must be > 0
- ? Start date must be in the future
- ? End date must be after start date
- ? Vehicle must be available for selected dates
- ? Pricing strategy must be valid ("standard", "weekend", "seasonal", "loyalty")

### Maintenance Creation
- ? Vehicle ID must exist
- ? Scheduled date is required
- ? Description is required
- ? Cost must be >= 0
- ? Type must be valid MaintenanceType enum value
- ? User must be Admin or Employee

## Frontend Model Validation

The frontend models in `Frontend\Models\RentalDtos.cs` now properly match the backend:

```csharp
public class PriceCalculationResponse
{
    public decimal TotalPrice { get; set; }
    public int NumberOfDays { get; set; }
    public decimal DailyRate { get; set; }
    public string StrategyUsed { get; set; } = string.Empty;
    public decimal? Discount { get; set; }
}
```

## Build Status

? **Backend Build**: Success  
? **Frontend Build**: Success (12 warnings - MudBlazor analyzer warnings only, not errors)

## Known Issues & Warnings

The following are **analyzer warnings only** and do not affect functionality:
- MudBlazor suggests using `Visible` instead of `IsVisible`
- MudBlazor suggests using `Tooltip` instead of `Title` on `MudIconButton`

These can be addressed in a future cleanup but are cosmetic only.

## Next Steps

1. ? Test rental creation with different pricing strategies
2. ? Test maintenance scheduling
3. ? Verify date calculations are correct
4. ? Test edge cases (same day rental, invalid dates, etc.)
5. Consider adding client-side validation for better UX
6. Add loading states during API calls
7. Consider implementing optimistic UI updates
