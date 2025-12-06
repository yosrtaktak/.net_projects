# ? Quick Setup: Categories in Vehicle Management

## 1. Insert Default Categories

Run this SQL in your database:

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

Or use the script:
```powershell
sqlcmd -S localhost -d CarRentalDB -i Backend/insert_default_categories.sql
```

## 2. Test It

```powershell
# Start application (if not running)
cd Frontend
dotnet run
```

1. Login as **Admin** or **Employee**
2. Navigate to `/vehicles/add`
3. **Expected**: Category dropdown shows all categories with descriptions and multipliers

## What You'll See

### Category Dropdown
```
?? Category ???????????????????????????
? ?? Economy                          ?
?    Budget-friendly vehicles...      ?
?    Price Multiplier: ×0.80         ?
???????????????????????????????????????
? ?? Compact                          ?
?    Small, efficient cars...         ?
?    Price Multiplier: ×1.00         ?
???????????????????????????????????????
? ?? SUV                              ?
?    Sport Utility Vehicles...        ?
?    Price Multiplier: ×1.50         ?
???????????????????????????????????????
```

### Helper Text (after selection)
```
Price multiplier: ×1.50 - Sport Utility Vehicles with spacious interiors
```

## Quick Verify

```sql
-- Check categories were inserted
SELECT Id, Name, PriceMultiplier, DisplayOrder, IsActive 
FROM Categories 
ORDER BY DisplayOrder;

-- Expected result:
-- 6 rows with Economy (0.8), Compact (1.0), Midsize (1.2), 
-- SUV (1.5), Luxury (2.0), Van (1.3)
```

## Features Working

? Categories from database in dropdown  
? Descriptions visible  
? Price multipliers shown  
? Ordered by DisplayOrder  
? Only active categories appear  
? Helper text updates on selection  

## Troubleshooting

### Empty Dropdown?
- Run the SQL script above
- Check categories are Active
- Refresh browser (Ctrl+F5)

### Category not showing?
- Must be marked as Active
- Name must be: Economy, Compact, Midsize, SUV, Luxury, Van, or Sports
- Case-insensitive

## Manage Categories

**URL**: `/admin/categories`

- Create new categories
- Edit existing ones
- Toggle active/inactive
- Set display order
- Update price multipliers

## Status

?? **READY TO USE**  
?? **BUILD SUCCESS**  
?? **INTEGRATED**  

---

**All categories now flow from database to vehicle forms!** ??
