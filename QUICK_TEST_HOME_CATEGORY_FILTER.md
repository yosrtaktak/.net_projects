# Quick Test - Home Page Category Filter

## ? 2-Minute Test

### Prerequisites
- Frontend running on https://localhost:7148
- At least one vehicle in each category (Economy, SUV, Luxury, Sports)

---

## Test Steps

### 1. Navigate to Home Page
```
URL: https://localhost:7148/
```

### 2. Test Economy Category
**Action:** Click the **"Economy"** category card

**? Expected Result:**
- URL changes to: `/vehicles/browse?category=Economy`
- Page shows "Browse Vehicles" page
- Category dropdown shows **"Economy"** selected
- Vehicle grid shows **ONLY Economy vehicles**
- Results count updates (e.g., "Showing 3 of 15 vehicles")

**? If Not Working:**
- Check browser console (F12) for errors
- Verify URL contains `?category=Economy`
- Clear browser cache and try again

---

### 3. Test SUV Category
**Action:** 
- Press browser Back button to return to home page
- Click the **"SUV"** category card

**? Expected Result:**
- URL: `/vehicles/browse?category=SUV`
- Category dropdown shows **"SUV"**
- Grid shows **ONLY SUV vehicles**

---

### 4. Test Luxury Category
**Action:** 
- Go back to home page
- Click the **"Luxury"** category card

**? Expected Result:**
- URL: `/vehicles/browse?category=Luxury`
- Category dropdown shows **"Luxury"**
- Grid shows **ONLY Luxury vehicles**

---

### 5. Test Sports Category
**Action:** 
- Go back to home page
- Click the **"Sports"** category card

**? Expected Result:**
- URL: `/vehicles/browse?category=Sports`
- Category dropdown shows **"Sports"**
- Grid shows **ONLY Sports vehicles**

---

### 6. Test Clear Filter
**Action:** 
- While on filtered page
- Click **"Clear Filters"** button

**? Expected Result:**
- Category dropdown clears
- Shows **ALL available vehicles**
- Results count shows full count

---

### 7. Test Manual Category Change
**Action:** 
- Change category dropdown to different category

**? Expected Result:**
- Filter updates immediately
- Vehicle grid refreshes
- Shows vehicles for newly selected category

---

## Visual Checklist

```
Home Page Category Cards:
?????????????????????????????????????????
? Economy ?   SUV   ? Luxury  ? Sports  ?
?   ??    ?   ??    ?   ??    ?   ???    ?
?????????????????????????????????????????

After Clicking Economy:
??????????????????????????????????????????
? Browse Vehicles Page                   ?
? Category: [Economy ?]  ? Selected!     ?
?                                        ?
? ??????????????????????                ?
? ?Economy?Economy?Economy?               ?
? ?Car 1 ?Car 2 ?Car 3 ?                ?
? ??????????????????????                ?
?                                        ?
? Showing 3 of 15 vehicles               ?
??????????????????????????????????????????
```

---

## Success Criteria

? All 4 category cards filter correctly
? URL updates with query parameter
? Category dropdown pre-selects correctly
? Vehicle grid shows only filtered vehicles
? Results count is accurate
? Clear Filters button works
? Manual filter changes work

---

## Common Issues

### Issue 1: No Vehicles Shown
**Cause:** No vehicles in that category
**Solution:** Check database has vehicles for each category:
```sql
SELECT Category, COUNT(*) as Count
FROM Vehicles
WHERE Status = 0  -- Available
GROUP BY Category;
```

### Issue 2: Filter Not Applied
**Cause:** Browser cache
**Solution:** Hard refresh with Ctrl+F5

### Issue 3: URL Doesn't Have Query Parameter
**Cause:** Home page navigation not working
**Solution:** Check Home.razor has:
```csharp
@onclick='() => NavigateToCategory("Economy")'
```

---

## Browser Console Check

Press **F12** and check:

**Network Tab:**
```
? GET /api/vehicles ? 200 OK
```

**Console Tab:**
```
No errors should appear
If errors, copy and investigate
```

**Application Tab:**
```
Check localStorage for any cached data
```

---

## If All Tests Pass

**?? Congratulations!** 

The home page category filtering is now working perfectly. Users can:
1. Click category cards on home page
2. Be taken to Browse Vehicles with filter applied
3. See only vehicles in that category
4. Clear filter or change to different category
5. Enjoy smooth filtering experience

---

## Next Steps

Test the complete flow:
1. ? Home ? Category ? Browse
2. ? Browse ? Select vehicle ? Rent Now
3. ? Complete booking
4. ? View in My Rentals

---

**Time to Test: 2 minutes** ??
**Difficulty: Easy** ??
**Impact: High** ??
