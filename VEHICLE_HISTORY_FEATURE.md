# Vehicle History Feature - Documentation 📋

## Overview

A comprehensive vehicle history tracking system has been added to the Car Rental System. This feature allows **administrators** to view complete historical data for each vehicle including:

- 📅 **Rental History** - All past and current rentals
- 🔧 **Maintenance History** - Complete maintenance records
- ⚠️ **Damage History** - All reported damages and repairs
- 📈 **Mileage Evolution** - Track mileage over time

## New Features

### 1. Vehicle History Page (`/vehicles/{id}/history`)

A dedicated page accessible only to administrators that displays:

#### Summary Dashboard
- Total number of rentals
- Total maintenance operations
- Total damage reports
- Total kilometers driven

#### Tabbed Interface
Four main tabs for different history categories:

1. **Rental History Tab**
   - Lists all rentals (past and present)
   - Shows customer information
   - Displays rental dates and costs
   - Shows distance driven per rental
   - Status badges for each rental

2. **Maintenance History Tab**
   - Complete maintenance records
   - Maintenance type (Routine, Repair, Inspection, Emergency)
   - Scheduled vs completed dates
   - Costs and descriptions
   - Status tracking

3. **Damage History Tab**
   - All reported damages
   - Severity levels (Minor, Moderate, Major, Critical)
   - Repair costs
   - Reported and repaired dates
   - Who reported the damage
   - Associated rental (if applicable)

4. **Mileage Evolution Tab**
   - Current vs initial mileage
   - Total kilometers driven
   - Average distance per rental
   - Timeline of mileage changes
   - Events tied to mileage (rental starts/ends, maintenance)

## New Database Entities

### VehicleDamage Entity
```csharp
public class VehicleDamage
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int? RentalId { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Description { get; set; }
    public DamageSeverity Severity { get; set; }  // Minor, Moderate, Major, Critical
    public decimal RepairCost { get; set; }
    public DateTime? RepairedDate { get; set; }
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
    public DamageStatus Status { get; set; }  // Reported, UnderRepair, Repaired, Unresolved
}
```

## API Endpoints

### Get Vehicle History
```
GET /api/vehicles/{id}/history
Authorization: Admin role required
```

**Response:**
```json
{
  "vehicle": { /* vehicle details */ },
  "rentals": [ /* rental history */ ],
  "maintenanceRecords": [ /* maintenance history */ ],
  "damageRecords": [ /* damage history */ ],
  "mileageEvolution": {
    "currentMileage": 15000,
    "initialMileage": 0,
    "totalMileageDriven": 15000,
    "averageMileagePerRental": 250,
    "dataPoints": [ /* timeline data */ ]
  }
}
```

## Files Created/Modified

### Backend

**New Files:**
- `Backend/Core/Entities/VehicleDamage.cs` - New entity for damage tracking

**Modified Files:**
- `Backend/Core/Entities/Vehicle.cs` - Added DamageRecords collection
- `Backend/Core/Interfaces/IVehicleRepository.cs` - Added GetByIdWithHistoryAsync method
- `Backend/Infrastructure/Repositories/VehicleRepository.cs` - Implemented history retrieval
- `Backend/Infrastructure/Data/CarRentalDbContext.cs` - Added VehicleDamages DbSet and configuration
- `Backend/Controllers/VehiclesController.cs` - Added GetVehicleHistory endpoint

### Frontend

**New Files:**
- `Frontend/Models/VehicleHistoryDtos.cs` - DTOs for vehicle history data
- `Frontend/Pages/VehicleHistory.razor` - Main history display page

**Modified Files:**
- `Frontend/Services/ApiService.cs` - Added GetVehicleHistoryAsync method
- `Frontend/Pages/ManageVehicles.razor` - Added History button for each vehicle

## How to Access

1. **Navigate to Manage Vehicles page** (`/vehicles/manage`)
2. As an **Admin**, you'll see a **History** button (🕒 icon) on each vehicle card
3. Click the History button to view complete vehicle history
4. Use tabs to navigate between different history types

## User Interface

### Color Coding

**Status Badges:**
- 🟢 **Available/Completed** - Green
- 🔵 **Reserved/Scheduled** - Blue
- 🟡 **Rented/In Progress** - Yellow
- ⚫ **Maintenance/Cancelled** - Gray
- 🔴 **Retired/Unresolved** - Red

**Severity Badges (Damage):**
- 🔵 **Minor** - Blue (bg-info)
- 🟡 **Moderate** - Yellow (bg-warning)
- 🔴 **Major** - Red (bg-danger)
- ⚫ **Critical** - Black (bg-dark)

### Icons Used
- 📅 `bi-calendar-check` - Rentals
- 🔧 `bi-tools` - Maintenance
- ⚠️ `bi-exclamation-triangle` - Damage
- 📈 `bi-graph-up` - Mileage
- 🕒 `bi-clock-history` - History

## Database Migration Required

After implementing these changes, you need to create and apply a migration:

```bash
# Navigate to Backend directory
cd Backend

# Create migration
dotnet ef migrations add AddVehicleDamageHistory

# Apply migration
dotnet ef database update
```

## Future Enhancements

Potential features to add:

1. **Export to PDF** - Export vehicle history as PDF report
2. **Charts/Graphs** - Visual representation of mileage evolution
3. **Compare Vehicles** - Compare history of multiple vehicles
4. **Notifications** - Alert when vehicle reaches maintenance milestones
5. **Damage Photos** - Upload and view damage photos
6. **Cost Analysis** - Analyze total cost of ownership per vehicle
7. **Predictive Maintenance** - Suggest maintenance based on mileage/time

## Benefits

### For Administrators
- ✅ Complete oversight of vehicle lifecycle
- ✅ Make informed decisions about vehicle retirement
- ✅ Track maintenance costs and patterns
- ✅ Identify problematic vehicles
- ✅ Monitor mileage accumulation
- ✅ Document damage claims

### For Business
- ✅ Better fleet management
- ✅ Cost tracking and optimization
- ✅ Maintenance planning
- ✅ Insurance claims documentation
- ✅ Vehicle depreciation tracking
- ✅ Data-driven decisions

## Testing Checklist

- [ ] Verify admin-only access to history page
- [ ] Test with vehicle that has no history
- [ ] Test with vehicle with complete history
- [ ] Verify all tabs load correctly
- [ ] Check mileage calculations are accurate
- [ ] Verify status badges display correctly
- [ ] Test navigation between tabs
- [ ] Verify responsive design on mobile
- [ ] Test with large datasets (many rentals/maintenances)
- [ ] Verify damage severity badges

## Support

For questions or issues with the Vehicle History feature, please refer to this documentation or contact the development team.

---
**Version:** 1.0  
**Last Updated:** Today  
**Feature Status:** ✅ Implemented and Ready for Testing
