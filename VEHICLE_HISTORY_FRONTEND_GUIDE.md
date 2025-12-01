# Vehicle History - Frontend Integration Guide

## 🎯 Quick Reference for Frontend Developers

This guide shows exactly what data each API endpoint returns and how to use it in your Blazor components.

---

## 📡 API Endpoints Summary

| Endpoint | Purpose | Returns |
|----------|---------|---------|
| `/api/vehicles/{id}/history` | Complete history | Vehicle + Rentals + Maintenances + Damages + Mileage |
| `/api/vehicles/{id}/rentals` | Rental history only | Rentals with statistics |
| `/api/vehicles/{id}/maintenances` | Maintenance history only | Maintenances with statistics |
| `/api/vehicles/{id}/damages` | Damage reports only | Damages with statistics |

---

## 📊 1. Rentals Endpoint

### API Call:
```csharp
var response = await Http.GetFromJsonAsync<RentalsResponse>(
    $"api/vehicles/{vehicleId}/rentals"
);
```

### Response Structure:
```csharp
public class RentalsResponse
{
    public int VehicleId { get; set; }
    
    // Statistics
    public int TotalRentals { get; set; }              // Total count
    public int CompletedRentals { get; set; }          // Only completed
    public decimal TotalRevenue { get; set; }          // Sum of completed costs
    public int TotalDistanceDriven { get; set; }       // Total KM
    
    // Rental list
    public List<RentalDto> Rentals { get; set; }
}

public class RentalDto
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public decimal TotalCost { get; set; }
    public int Status { get; set; }                     // 0=Reserved, 1=Active, 2=Completed, 3=Cancelled
    public int? StartMileage { get; set; }
    public int? EndMileage { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Customer details
    public CustomerDto Customer { get; set; }
    
    // Calculated fields
    public int? DistanceDriven { get; set; }           // EndMileage - StartMileage
    public int DaysRented { get; set; }                // Duration
}

public class CustomerDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}
```

### Display in UI:
```razor
@if (rentalsData != null)
{
    <!-- Summary Cards -->
    <div class="stats-grid">
        <div class="stat-card">
            <h4>Total Rentals</h4>
            <p class="stat-value">@rentalsData.TotalRentals</p>
        </div>
        <div class="stat-card">
            <h4>Total Revenue</h4>
            <p class="stat-value">$@rentalsData.TotalRevenue.ToString("N2")</p>
        </div>
        <div class="stat-card">
            <h4>Distance Driven</h4>
            <p class="stat-value">@rentalsData.TotalDistanceDriven km</p>
        </div>
    </div>

    <!-- Rental List -->
    @foreach (var rental in rentalsData.Rentals)
    {
        <div class="rental-card">
            <h5>@rental.Customer.FirstName @rental.Customer.LastName</h5>
            <p>@rental.StartDate.ToString("MMM dd, yyyy") - @rental.EndDate.ToString("MMM dd, yyyy")</p>
            <p>Distance: @rental.DistanceDriven km</p>
            <p>Cost: $@rental.TotalCost</p>
            <span class="badge">@GetStatusBadge(rental.Status)</span>
        </div>
    }
}
```

---

## 🔧 2. Maintenances Endpoint

### API Call:
```csharp
var response = await Http.GetFromJsonAsync<MaintenancesResponse>(
    $"api/vehicles/{vehicleId}/maintenances"
);
```

### Response Structure:
```csharp
public class MaintenancesResponse
{
    public int VehicleId { get; set; }
    
    // Statistics
    public int TotalMaintenances { get; set; }
    public int CompletedMaintenances { get; set; }
    public int ScheduledMaintenances { get; set; }
    public int InProgressMaintenances { get; set; }
    public int OverdueMaintenances { get; set; }
    public decimal TotalMaintenanceCost { get; set; }
    
    // Maintenance list
    public List<MaintenanceDto> Maintenances { get; set; }
}

public class MaintenanceDto
{
    public int Id { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string Description { get; set; }
    public decimal Cost { get; set; }
    public int Type { get; set; }                      // 0=Routine, 1=Repair, 2=Inspection, 3=Emergency
    public int Status { get; set; }                    // 0=Scheduled, 1=InProgress, 2=Completed, 3=Cancelled
    
    // Calculated fields
    public int? DaysToComplete { get; set; }
    public bool IsOverdue { get; set; }
    public string TypeName { get; set; }               // "Routine", "Repair", etc.
    public string StatusName { get; set; }             // "Completed", "Scheduled", etc.
}
```

### Display in UI:
```razor
@if (maintenancesData != null)
{
    <!-- Summary Cards -->
    <div class="stats-grid">
        <div class="stat-card">
            <h4>Total Maintenances</h4>
            <p class="stat-value">@maintenancesData.TotalMaintenances</p>
        </div>
        <div class="stat-card">
            <h4>Completed</h4>
            <p class="stat-value">@maintenancesData.CompletedMaintenances</p>
        </div>
        <div class="stat-card">
            <h4>Scheduled</h4>
            <p class="stat-value">@maintenancesData.ScheduledMaintenances</p>
        </div>
        <div class="stat-card">
            <h4>Total Cost</h4>
            <p class="stat-value">$@maintenancesData.TotalMaintenanceCost.ToString("N2")</p>
        </div>
    </div>

    <!-- Maintenance List -->
    @foreach (var maintenance in maintenancesData.Maintenances)
    {
        <div class="maintenance-card @(maintenance.IsOverdue ? "overdue" : "")">
            <div class="maintenance-header">
                <span class="type-badge">@maintenance.TypeName</span>
                <span class="status-badge">@maintenance.StatusName</span>
            </div>
            <p class="description">@maintenance.Description</p>
            <p>Scheduled: @maintenance.ScheduledDate.ToString("MMM dd, yyyy")</p>
            @if (maintenance.CompletedDate.HasValue)
            {
                <p>Completed: @maintenance.CompletedDate.Value.ToString("MMM dd, yyyy")</p>
            }
            <p class="cost">Cost: $@maintenance.Cost</p>
            @if (maintenance.IsOverdue)
            {
                <p class="overdue-warning">⚠️ Overdue!</p>
            }
        </div>
    }
}
```

---

## 🛠️ 3. Damages Endpoint

### API Call:
```csharp
var response = await Http.GetFromJsonAsync<DamagesResponse>(
    $"api/vehicles/{vehicleId}/damages"
);
```

### Response Structure:
```csharp
public class DamagesResponse
{
    public int VehicleId { get; set; }
    
    // Statistics
    public int TotalDamages { get; set; }
    public int RepairedDamages { get; set; }
    public int UnderRepairDamages { get; set; }
    public int UnresolvedDamages { get; set; }
    public decimal TotalRepairCost { get; set; }
    
    // Severity breakdown
    public int MinorDamages { get; set; }
    public int ModerateDamages { get; set; }
    public int MajorDamages { get; set; }
    public int CriticalDamages { get; set; }
    
    // Damage list
    public List<DamageDto> Damages { get; set; }
}

public class DamageDto
{
    public int Id { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Description { get; set; }
    public int Severity { get; set; }                  // 0=Minor, 1=Moderate, 2=Major, 3=Critical
    public decimal RepairCost { get; set; }
    public DateTime? RepairedDate { get; set; }
    public string? ReportedBy { get; set; }
    public string? ImageUrl { get; set; }
    public int Status { get; set; }                    // 0=Reported, 1=UnderRepair, 2=Repaired, 3=Unresolved
    public int? RentalId { get; set; }
    
    // Calculated fields
    public string SeverityName { get; set; }           // "Minor", "Moderate", etc.
    public string StatusName { get; set; }             // "Repaired", "UnderRepair", etc.
    public int? DaysToRepair { get; set; }
    public bool IsUnderRepair { get; set; }
    public RentalDto? RelatedRental { get; set; }      // If linked to a rental
}
```

### Display in UI:
```razor
@if (damagesData != null)
{
    <!-- Summary Cards -->
    <div class="stats-grid">
        <div class="stat-card">
            <h4>Total Damages</h4>
            <p class="stat-value">@damagesData.TotalDamages</p>
        </div>
        <div class="stat-card">
            <h4>Repaired</h4>
            <p class="stat-value">@damagesData.RepairedDamages</p>
        </div>
        <div class="stat-card">
            <h4>Under Repair</h4>
            <p class="stat-value">@damagesData.UnderRepairDamages</p>
        </div>
        <div class="stat-card">
            <h4>Total Cost</h4>
            <p class="stat-value">$@damagesData.TotalRepairCost.ToString("N2")</p>
        </div>
    </div>

    <!-- Severity Breakdown -->
    <div class="severity-chart">
        <div class="severity-bar">
            <span>Minor: @damagesData.MinorDamages</span>
            <span>Moderate: @damagesData.ModerateDamages</span>
            <span>Major: @damagesData.MajorDamages</span>
            <span>Critical: @damagesData.CriticalDamages</span>
        </div>
    </div>

    <!-- Damage List -->
    @foreach (var damage in damagesData.Damages)
    {
        <div class="damage-card severity-@damage.SeverityName.ToLower()">
            <div class="damage-header">
                <span class="severity-badge">@damage.SeverityName</span>
                <span class="status-badge">@damage.StatusName</span>
            </div>
            
            @if (!string.IsNullOrEmpty(damage.ImageUrl))
            {
                <img src="@damage.ImageUrl" alt="Damage photo" class="damage-image" />
            }
            
            <p class="description">@damage.Description</p>
            <p>Reported: @damage.ReportedDate.ToString("MMM dd, yyyy")</p>
            <p>Reported by: @damage.ReportedBy</p>
            <p class="cost">Repair Cost: $@damage.RepairCost</p>
            
            @if (damage.RepairedDate.HasValue)
            {
                <p>✅ Repaired: @damage.RepairedDate.Value.ToString("MMM dd, yyyy")</p>
                <p>Days to repair: @damage.DaysToRepair</p>
            }
            else if (damage.IsUnderRepair)
            {
                <p class="under-repair">🔧 Currently under repair</p>
            }
            
            @if (damage.RentalId.HasValue)
            {
                <p class="rental-link">
                    🚗 Occurred during rental #@damage.RentalId
                </p>
            }
        </div>
    }
}
```

---

## 🎨 CSS Styling Examples

### Status Badges:
```css
.badge {
    padding: 4px 12px;
    border-radius: 12px;
    font-size: 0.85rem;
    font-weight: 600;
}

/* Rental Status */
.badge.completed { background: #d4edda; color: #155724; }
.badge.active { background: #fff3cd; color: #856404; }
.badge.cancelled { background: #f8d7da; color: #721c24; }

/* Maintenance Status */
.status-badge.completed { background: #28a745; color: white; }
.status-badge.scheduled { background: #17a2b8; color: white; }
.status-badge.in-progress { background: #ffc107; color: black; }
.status-badge.overdue { background: #dc3545; color: white; }

/* Damage Severity */
.severity-badge.minor { background: #ffc107; color: black; }
.severity-badge.moderate { background: #fd7e14; color: white; }
.severity-badge.major { background: #dc3545; color: white; }
.severity-badge.critical { background: #721c24; color: white; }

/* Damage Status */
.status-badge.repaired { background: #28a745; color: white; }
.status-badge.underrepair { background: #17a2b8; color: white; }
.status-badge.unresolved { background: #6c757d; color: white; }
```

---

## 🔄 Complete Integration Example

### VehicleHistory.razor Component:

```razor
@page "/vehicle-history/{VehicleId:int}"
@inject HttpClient Http
@inject NavigationManager Navigation

<h3>Vehicle History</h3>

<div class="tabs">
    <button class="@(activeTab == "rentals" ? "active" : "")" 
            @onclick="() => LoadTab(\"rentals\")">
        Rentals (@rentalsData?.TotalRentals ?? 0)
    </button>
    <button class="@(activeTab == "maintenances" ? "active" : "")" 
            @onclick="() => LoadTab(\"maintenances\")">
        Maintenances (@maintenancesData?.TotalMaintenances ?? 0)
    </button>
    <button class="@(activeTab == "damages" ? "active" : "")" 
            @onclick="() => LoadTab(\"damages\")">
        Damages (@damagesData?.TotalDamages ?? 0)
    </button>
</div>

<div class="tab-content">
    @if (activeTab == "rentals" && rentalsData != null)
    {
        <!-- Rentals UI here -->
    }
    else if (activeTab == "maintenances" && maintenancesData != null)
    {
        <!-- Maintenances UI here -->
    }
    else if (activeTab == "damages" && damagesData != null)
    {
        <!-- Damages UI here -->
    }
    else if (isLoading)
    {
        <p>Loading...</p>
    }
</div>

@code {
    [Parameter] public int VehicleId { get; set; }
    
    private string activeTab = "rentals";
    private bool isLoading = false;
    
    private RentalsResponse? rentalsData;
    private MaintenancesResponse? maintenancesData;
    private DamagesResponse? damagesData;
    
    protected override async Task OnInitializedAsync()
    {
        await LoadTab("rentals");
    }
    
    private async Task LoadTab(string tab)
    {
        activeTab = tab;
        isLoading = true;
        
        try
        {
            switch (tab)
            {
                case "rentals":
                    if (rentalsData == null)
                        rentalsData = await Http.GetFromJsonAsync<RentalsResponse>(
                            $"api/vehicles/{VehicleId}/rentals");
                    break;
                    
                case "maintenances":
                    if (maintenancesData == null)
                        maintenancesData = await Http.GetFromJsonAsync<MaintenancesResponse>(
                            $"api/vehicles/{VehicleId}/maintenances");
                    break;
                    
                case "damages":
                    if (damagesData == null)
                        damagesData = await Http.GetFromJsonAsync<DamagesResponse>(
                            $"api/vehicles/{VehicleId}/damages");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading {tab}: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

---

## ✅ Implementation Checklist

- [ ] Create DTO classes for responses
- [ ] Add authorization header to HTTP client
- [ ] Implement tab navigation
- [ ] Display summary statistics cards
- [ ] Render list items with proper styling
- [ ] Add status badges with colors
- [ ] Format dates properly
- [ ] Format currency values
- [ ] Handle loading states
- [ ] Handle error states
- [ ] Add empty state messages
- [ ] Test with real data
- [ ] Verify calculations are correct
- [ ] Test responsive design

---

## 🚀 Testing

1. **Start Backend:** `cd Backend && dotnet run`
2. **Test with Vehicle ID 1** (Toyota Corolla has seeded data)
3. **Verify each endpoint** returns correct statistics
4. **Check UI** displays all data properly

---

**Ready to implement!** 🎉

All backend endpoints are now available. Just restart your backend and the new endpoints will be active.
