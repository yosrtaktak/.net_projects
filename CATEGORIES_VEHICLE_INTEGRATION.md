# Integration: Categories in Vehicle Management ?

## Overview

The vehicle management system now uses the categories from the **Categories** table in the database when adding or editing vehicles. This provides a centralized way to manage vehicle categories with descriptions and price multipliers.

## What Changed

### 1. Frontend Vehicle Enum Updated
**File**: `Frontend/Models/VehicleDtos.cs`

Added `Midsize` to the `VehicleCategory` enum to match the database categories:

```csharp
public enum VehicleCategory
{
    Economy,
    Compact,
    Midsize,    // ? Added
    SUV,
    Luxury,
    Sports,
    Van
}
```

### 2. ManageVehicle Page Enhanced
**File**: `Frontend/Pages/ManageVehicle.razor`

**Changes:**
- ? Loads categories from database on page init
- ? Shows only active categories in dropdown
- ? Displays category description and price multiplier
- ? Maps database category names to enum values
- ? Shows warning if no categories exist (with link to create them)
- ? Helper text shows selected category details

**New Features in Category Dropdown:**
- Category name
- Description (if available)
- Price multiplier (e.g., "×0.80", "×1.50")
- Color-coded icons
- Ordered by DisplayOrder from database

### 3. Database Setup Script Created
**File**: `Backend/insert_default_categories.sql`

Created SQL script to populate initial categories:
- Economy (×0.8)
- Compact (×1.0)
- Midsize (×1.2)
- SUV (×1.5)
- Luxury (×2.0)
- Van (×1.3)

## Setup Instructions

### Step 1: Insert Default Categories into Database

```powershell
# Option A: Using SQL Server Management Studio (SSMS)
# 1. Open Backend/insert_default_categories.sql
# 2. Connect to your database
# 3. Execute the script

# Option B: Using Command Line
sqlcmd -S localhost -d CarRentalDB -i Backend/insert_default_categories.sql
```

Or run this SQL directly:

```sql
INSERT INTO Categories (Name, Description, IsActive, DisplayOrder, PriceMultiplier, IconUrl, CreatedAt)
VALUES 
('Economy', 'Budget-friendly vehicles with excellent fuel efficiency', 1, 0, 0.8, NULL, GETDATE()),
('Compact', 'Small, efficient cars perfect for city driving', 1, 1, 1.0, NULL, GETDATE()),
('Midsize', 'Mid-range sedans with comfortable seating', 1, 2, 1.2, NULL, GETDATE()),
('SUV', 'Sport Utility Vehicles with spacious interiors', 1, 3, 1.5, NULL, GETDATE()),
('Luxury', 'Premium vehicles with high-end features', 1, 4, 2.0, NULL, GETDATE()),
('Van', 'Large capacity vehicles for groups and families', 1, 5, 1.3, NULL, GETDATE());
```

### Step 2: Restart Frontend (if running)

```powershell
# Stop frontend (Ctrl+C)
# Then restart:
cd Frontend
dotnet run
```

### Step 3: Test the Integration

1. Login as **Admin** or **Employee**
2. Navigate to `/vehicles/add`
3. **Expected**: Category dropdown loads from database
4. **Expected**: Each category shows description and multiplier
5. Select a category and create a vehicle

## How It Works

### Category Loading Flow

```
Page Load
    ?
LoadCategories() - Calls API
    ?
ApiService.GetCategoriesAsync(activeOnly: true)
    ?
GET /api/categories?activeOnly=true
    ?
Returns active categories ordered by DisplayOrder
    ?
ParseCategoryName() maps DB names to enum values
    ?
Display in dropdown with descriptions
```

### Category Selection Example

When you select "SUV" category:

```
Category Dropdown Shows:
???????????????????????????????????????????
? ?? SUV                                  ?
?    Sport Utility Vehicles with spacious?
?    interiors                           ?
?    Price Multiplier: ×1.50             ?
???????????????????????????????????????????

Helper Text Updates:
"Price multiplier: ×1.50 - Sport Utility Vehicles with spacious interiors"
```

## Category Name Mapping

The system maps database category names to enum values:

| Database Category | Enum Value              |
|-------------------|-------------------------|
| Economy           | VehicleCategory.Economy |
| Compact           | VehicleCategory.Compact |
| Midsize           | VehicleCategory.Midsize |
| SUV               | VehicleCategory.SUV     |
| Luxury            | VehicleCategory.Luxury  |
| Van               | VehicleCategory.Van     |
| Sports            | VehicleCategory.Sports  |

**Note**: Mapping is case-insensitive.

## Features

### ? Database-Driven Categories
- Categories loaded from database, not hardcoded
- Only active categories appear in dropdown
- Ordered by DisplayOrder field
- Real-time updates when categories change

### ? Rich Information Display
- Category name with icon
- Description (if provided)
- Price multiplier badge
- Color-coded for visual distinction

### ? Validation
- Warning if no categories exist
- Link to create categories
- Prevents vehicle creation without categories

### ? Smart Mapping
- Database category names map to enum values
- Case-insensitive matching
- Handles missing enum values gracefully

## UI Screenshots

### Add Vehicle - Category Section
```
???????????????????????????????????????????????????????????
? ?? Category & Pricing                                   ?
???????????????????????????????????????????????????????????
?                                                         ?
? ?? Category ?????????????????????????????????????????? ?
? ?  Economy                                          ?? ?
? ????????????????????????????????????????????????????? ?
?  Price multiplier: ×0.80 - Budget-friendly vehicles... ?
?                                                         ?
? ?? Daily Rate ???????????????????????????????????????? ?
? ? $ 50.00                                            ? ?
? ????????????????????????????????????????????????????? ?
???????????????????????????????????????????????????????????
```

### Category Dropdown Expanded
```
?? Category ???????????????????????????????
? ?? Economy                              ?
?    Budget-friendly vehicles...          ?
?    Price Multiplier: ×0.80             ?
???????????????????????????????????????????
? ?? Compact                              ?
?    Small, efficient cars...             ?
?    Price Multiplier: ×1.00             ?
???????????????????????????????????????????
? ?? Midsize                              ?
?    Mid-range sedans...                  ?
?    Price Multiplier: ×1.20             ?
???????????????????????????????????????????
? ?? SUV                                  ?
?    Sport Utility Vehicles...            ?
?    Price Multiplier: ×1.50             ?
???????????????????????????????????????????
? ?? Luxury                               ?
?    Premium vehicles...                  ?
?    Price Multiplier: ×2.00             ?
???????????????????????????????????????????
? ?? Van                                  ?
?    Large capacity vehicles...           ?
?    Price Multiplier: ×1.30             ?
???????????????????????????????????????????
```

## Testing Checklist

### Test 1: View Categories in Dropdown
- [  ] Login as Admin/Employee
- [  ] Navigate to `/vehicles/add`
- [  ] Category dropdown shows database categories
- [  ] Each category shows description
- [  ] Each category shows price multiplier
- [  ] Categories ordered by DisplayOrder

### Test 2: Create Vehicle with Category
- [  ] Select "Economy" category
- [  ] Helper text shows "×0.80 - Budget-friendly..."
- [  ] Fill other fields
- [  ] Submit form
- [  ] Vehicle created with correct category

### Test 3: Edit Vehicle Category
- [  ] Navigate to `/vehicles/edit/1`
- [  ] Current category is pre-selected
- [  ] Change to "Luxury" category
- [  ] Helper text updates to show ×2.00
- [  ] Save changes
- [  ] Vehicle updated successfully

### Test 4: No Categories Warning
- [  ] Delete all categories (or deactivate all)
- [  ] Navigate to `/vehicles/add`
- [  ] Warning message appears
- [  ] Link to "Create categories first" works
- [  ] Cannot submit form

### Test 5: Inactive Categories Hidden
- [  ] Create a category and mark inactive
- [  ] Navigate to `/vehicles/add`
- [  ] Inactive category does NOT appear in dropdown
- [  ] Only active categories visible

## Manage Categories

To add or edit categories:

1. **Navigate to**: `/admin/categories`
2. **Add Category**: Click "Add Category" button
3. **Fill Form**:
   - Name (required) - e.g., "Sports"
   - Description - e.g., "High-performance vehicles"
   - Display Order - e.g., 6
   - Price Multiplier - e.g., 2.5
   - Active - Toggle ON
4. **Save**: Click "Create"
5. **Result**: New category appears in vehicle dropdowns immediately

## Troubleshooting

### Issue: Dropdown is empty
**Cause**: No active categories in database

**Solution**:
1. Run `Backend/insert_default_categories.sql`
2. Or create categories at `/admin/categories`
3. Ensure categories are marked as Active

### Issue: Category not appearing in dropdown
**Cause**: Category is inactive or name doesn't map to enum

**Solution**:
1. Check category is Active in `/admin/categories`
2. Verify category name matches one of: Economy, Compact, Midsize, SUV, Luxury, Van, Sports
3. Names are case-insensitive but must match exactly

### Issue: "Price multiplier" shows in helper text but not in selection
**Cause**: Normal behavior - detailed info shows after selection

**Solution**: This is expected. Dropdown shows full details, helper text shows selected category info.

### Issue: Warning "No categories found"
**Cause**: Database has no categories or all are inactive

**Solution**:
```sql
-- Check existing categories
SELECT * FROM Categories;

-- If empty, run:
-- Backend/insert_default_categories.sql

-- If all inactive, activate them:
UPDATE Categories SET IsActive = 1;
```

## Benefits of This Integration

### 1. Centralized Management
- One place to manage all categories
- No need to update code to add categories
- Consistent across entire application

### 2. Flexibility
- Add/remove categories without code changes
- Update descriptions and multipliers easily
- Control visibility with Active toggle

### 3. Better UX
- Users see descriptions in dropdown
- Price multipliers visible upfront
- Clear categorization of vehicles

### 4. Data Integrity
- Categories validated against database
- Prevents orphaned category references
- Consistent pricing strategies

## Next Steps (Optional Enhancements)

### 1. Add "Sports" Category
Currently "Sports" is in the enum but not in default categories:

```sql
INSERT INTO Categories (Name, Description, IsActive, DisplayOrder, PriceMultiplier, CreatedAt)
VALUES ('Sports', 'High-performance sports cars', 1, 6, 2.5, GETDATE());
```

### 2. Sync Enum with Database
Consider migrating fully to database-driven categories and removing the enum dependency.

### 3. Category Icons
Upload actual icons for each category instead of using the default icon.

### 4. Category Analytics
Show how many vehicles are in each category on the Categories page.

### 5. Bulk Category Assignment
Allow selecting multiple vehicles and changing their category in bulk.

## Files Modified

1. **Frontend/Models/VehicleDtos.cs** - Added Midsize to enum
2. **Frontend/Pages/ManageVehicle.razor** - Load categories from database, enhanced dropdown
3. **Backend/insert_default_categories.sql** (NEW) - SQL script to populate categories

## Build Status

? **Frontend**: Builds successfully (8 non-critical warnings)
? **Backend**: No changes needed (already has Categories API)

## Summary

The vehicle management system now seamlessly integrates with the category management system:

- ? Categories loaded from database
- ? Rich information display (description, multiplier)
- ? Only active categories shown
- ? Ordered by DisplayOrder
- ? Warning if no categories exist
- ? Helper text shows selected category details
- ? Validation prevents vehicles without categories

**Status**: Ready to use! Run the SQL script to populate categories and start adding vehicles.
