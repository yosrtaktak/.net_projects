# Frontend Update Guide - Vehicle Category Migration

## Overview
The frontend needs to be updated to work with the new Vehicle-Category relationship where vehicles are linked to categories via CategoryId instead of using a VehicleCategory enum.

## ✅ What Was Completed

### 1. Updated Models
**File**: `Frontend/Models/VehicleDtos.cs`
- ❌ Removed `VehicleCategory` enum
- ✅ Added `CategoryId` (int) property to Vehicle model
- ✅ Added `Category` navigation property (CategoryModel?)
- ✅ Added `CategoryName` helper property for backward compatibility
- ✅ Updated `CreateVehicleRequest` to use `CategoryId` instead of enum

### 2. Updated ManageVehicles.razor
- ✅ Changed category filter from enum dropdown to category ID dropdown
- ✅ Load categories from API via `GetCategoriesAsync()`
- ✅ Updated `ApplyFilters()` to use `CategoryId` instead of enum
- ✅ Changed `GetCategoryColor()` to accept string parameter instead of enum
- ✅ Updated display to use `vehicle.CategoryName` instead of `vehicle.Category`

## 🔧 Pages That Still Need Updates

### High Priority - Dashboard/Management Pages

#### 1. **ManageVehicle.razor** (Add/Edit Vehicle Page)
**Status**: ⚠️ Partially Updated
**Needs**:
- Currently uses `VehicleCategory` enum in `VehicleFormModel`
- Has `ParseCategoryName()` method that converts DB category names to enum
- Has `GetCategoryColor(VehicleCategory)` method using enum
- **Fix**: Update `VehicleFormModel` to use `int CategoryId` instead of `VehicleCategory Category`
- **Fix**: Change category selection to work with CategoryId
- **Fix**: Update form submission to send CategoryId
- **Fix**: Update `GetCategoryColor()` to accept string

**Code Changes Needed**:
```csharp
// OLD:
public class VehicleFormModel
{
    public VehicleCategory Category { get; set; }
}

// NEW:
public class VehicleFormModel
{
    public int CategoryId { get; set; }
}
```

#### 2. **Vehicles.razor** (Vehicle List Page)
**Status**: ❌ Not Updated
**Needs**:
- Uses `Enum.GetValues<VehicleCategory>()` for category filter buttons
- Uses `vehicle.Category` enum comparison in `FilterByCategory()`
- Uses `GetCategoryColor(VehicleCategory)` method
- **Fix**: Load categories from API
- **Fix**: Replace enum iteration with category list iteration
- **Fix**: Update filtering logic to use CategoryId
- **Fix**: Update `GetCategoryColor()` to accept string

#### 3. **BrowseVehicles.razor** (Customer Browse Page)
**Status**: ❌ Not Updated
**Needs**:
- Uses `VehicleCategory?` for filter
- Uses `Enum.GetValues<VehicleCategory>()` for dropdown
- **Fix**: Change to use `int? selectedCategoryId`
- **Fix**: Load and display categories from API
- **Fix**: Update filtering to use CategoryId

###Middle Priority - Display Pages

#### 4. **VehicleDetails.razor**
**Status**: ✅ Should Work (uses display only)
**Note**: Should automatically work with `vehicle.CategoryName` property

#### 5. **AdminDashboard.razor**
**Status**: ✅ Already Works
**Note**: Doesn't display individual vehicle categories

### Low Priority - Supporting Pages

#### 6. **Reports.razor**
**Status**: ⚠️ Needs Verification
**Needs**:
- Check if vehicle category display works with new structure
- Backend ReportService already updated to use `Category.Name`

## 📝 Step-by-Step Fix Instructions

### Fix ManageVehicle.razor

1. **Update VehicleFormModel**:
```csharp
public class VehicleFormModel
{
    // ... existing properties
    public int CategoryId { get; set; }  // Changed from VehicleCategory Category
    // ... existing properties
}
```

2. **Remove ParseCategoryName and related enum logic**

3. **Update LoadVehicle method**:
```csharp
vehicleModel = new VehicleFormModel
{
    // ... existing assignments
    CategoryId = vehicle.CategoryId,  // Changed from Category = vehicle.Category
    // ... existing assignments
};
```

4. **Update HandleSubmit methods**:
```csharp
var vehicle = new Vehicle
{
    // ... existing properties
    CategoryId = vehicleModel.CategoryId,  // Changed from Category = vehicleModel.Category
    // ... existing properties
};
```

5. **Update GetCategoryColor**:
```csharp
private Color GetCategoryColor(string categoryName)
{
    return categoryName.ToLower() switch
    {
        "economy" => Color.Info,
        "compact" => Color.Primary,
        "midsize" => Color.Success,
        "suv" => Color.Success,
        "luxury" => Color.Warning,
        "sports" => Color.Error,
        "van" => Color.Secondary,
        _ => Color.Default
    };
}
```

### Fix Vehicles.razor

1. **Add category loading**:
```csharp
private List<CategoryModel> categories = new();
private int? selectedCategoryId;

protected override async Task OnInitializedAsync()
{
    // ... existing code
    categories = await ApiService.GetCategoriesAsync(activeOnly: true);
    await LoadVehicles();
}
```

2. **Replace enum iteration with category list**:
```html
<div class="btn-group flex-wrap mt-2" role="group">
    @foreach (var category in categories)
    {
        <button type="button" 
                class="btn btn-sm @(selectedCategoryId == category.Id ? "btn-primary" : "btn-outline-primary")" 
                @onclick="() => FilterByCategoryId(category.Id)">
            @category.Name
        </button>
    }
</div>
```

3. **Update filtering methods**:
```csharp
private async Task FilterByCategoryId(int? categoryId)
{
    selectedCategoryId = categoryId;
    isLoading = true;

    if (categoryId.HasValue)
    {
        vehicles = await ApiService.GetVehiclesByCategoryIdAsync(categoryId.Value);
    }
    else
    {
        vehicles = await ApiService.GetVehiclesAsync();
    }

    ApplyFilters();
    isLoading = false;
}
```

### Fix BrowseVehicles.razor

1. **Update state variables**:
```csharp
private int? selectedCategoryId;
private List<CategoryModel> categories = new();
```

2. **Load categories**:
```csharp
protected override async Task OnInitializedAsync()
{
    categories = await ApiService.GetCategoriesAsync(activeOnly: true);
    // ... rest of initialization
}
```

3. **Update category filter dropdown**:
```html
<MudSelect T="int?" 
           Label="Category" 
           Value="@selectedCategoryId" 
           ValueChanged="@((int? value) => OnCategoryChanged(value))"
           Clearable="true">
    @foreach (var category in categories)
    {
        <MudSelectItem T="int?" Value="@category.Id">@category.Name</MudSelectItem>
    }
</MudSelect>
```

4. **Update filtering**:
```csharp
private void OnCategoryChanged(int? value)
{
    selectedCategoryId = value;
    ApplyFilters();
    StateHasChanged();
}

private void ApplyFilters()
{
    var filtered = vehicles.AsEnumerable();

    if (selectedCategoryId.HasValue)
    {
        filtered = filtered.Where(v => v.CategoryId == selectedCategoryId.Value);
    }
    
    // ... rest of filters
    filteredVehicles = filtered.ToList();
}
```

## 🎯 Testing Checklist

After making updates, test:

- [ ] **ManageVehicles Page**: Can see all vehicles with their category names
- [ ] **Add Vehicle**: Can select category and create vehicle
- [ ] **Edit Vehicle**: Can see and change vehicle category
- [ ] **Filter by Category**: Category dropdown shows all categories
- [ ] **Search**: Search still works after filtering
- [ ] **Browse Vehicles (Customer)**: Category filter works
- [ ] **Vehicle Details**: Category displays correctly
- [ ] **Reports**: Vehicles by category shows correct data

## 🚨 Common Errors to Watch For

1. **"VehicleCategory does not exist"**: Remove all enum references
2. **Null reference on Category**: Ensure API returns Category navigation property
3. **Category dropdown empty**: Check that categories are loaded from API
4. **Filter not working**: Verify using CategoryId not enum for comparisons

## 📦 API Service Methods Needed

Make sure these exist in your `IApiService`:

```csharp
Task<List<CategoryModel>> GetCategoriesAsync(bool activeOnly = false);
Task<List<Vehicle>> GetVehiclesByCategoryIdAsync(int categoryId);
```

## ✅ Migration Complete When...

- [ ] No compilation errors in Frontend project
- [ ] All pages load without JavaScript errors
- [ ] Vehicle category displays correctly everywhere
- [ ] Category filtering works on all pages
- [ ] Can create/edit vehicles with categories
- [ ] Dashboard shows vehicle statistics correctly

## 📝 Notes

- The backend is already fully migrated and working
- Categories are seeded in the database with migration `20251206175435_LinkVehicleToCategories`
- All 7 categories (Economy, Compact, Midsize, SUV, Luxury, Van, Sports) are available
- Category management page at `/admin/categories` allows adding/editing categories
