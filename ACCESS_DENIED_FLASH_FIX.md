# Access Denied Flash Fix

## Problem
When navigating to admin/employee pages, an "Access Denied" error was briefly visible before disappearing. This created a poor user experience with flickering error messages.

## Root Cause
The issue occurred because:
1. The page rendered immediately with default values (`canViewMaintenances = false`, `isAuthorized = false`)
2. The authorization check in `OnInitializedAsync` was asynchronous
3. The UI checked authorization **before** checking if loading was complete
4. This caused the error message to show briefly until the async check completed

**Timeline:**
```
0ms:   Component renders
       ├─ isLoading = true (initially)
       ├─ canViewMaintenances = false (default)
       └─ UI shows "Access Denied" ❌

50ms:  Auth check starts (async)

100ms: Auth check completes
       ├─ canViewMaintenances = true
       └─ UI updates to show correct content
```

## Solution
Fixed by implementing two key changes:

### 1. Initialize `isLoading = true`
Set loading state to true at the start of `OnInitializedAsync`:

```csharp
protected override async Task OnInitializedAsync()
{
    isLoading = true;  // ← Added this
    
    var role = await AuthService.GetRoleAsync();
    canViewMaintenances = role == "Admin" || role == "Employee";
    isAdmin = role == "Admin";

    if (!canViewMaintenances)
    {
        isLoading = false;
        NavigationManager.NavigateTo("/");
        return;
    }

    await LoadData();
}
```

### 2. Check Loading State First in UI
Reordered the conditional checks to show loading spinner before authorization check:

**❌ Before (Incorrect Order):**
```razor
@if (!canViewMaintenances)
{
    <MudAlert>Access Denied</MudAlert>
}
else
{
    @if (isLoading)
    {
        <MudProgressLinear />
    }
    else
    {
        <!-- Content -->
    }
}
```

**✅ After (Correct Order):**
```razor
@if (isLoading)
{
    <MudProgressLinear Indeterminate="true" Color="Color.Primary" />
}
else if (!canViewMaintenances)
{
    <MudAlert Severity="Severity.Error">
        <strong>Access Denied</strong> - Admin or Employee privileges required.
    </MudAlert>
}
else
{
    <!-- Content -->
}
```

## Fixed Timeline
```
0ms:   Component renders
       ├─ isLoading = true ✅
       └─ UI shows loading spinner ✅

50ms:  Auth check starts (async)

100ms: Auth check completes
       ├─ isLoading = false
       ├─ canViewMaintenances = true/false
       └─ UI shows correct content OR access denied (no flicker!)
```

## Files Fixed

### 1. **Frontend/Pages/Maintenances.razor**
- ✅ Reordered UI checks (loading → auth → content)
- ✅ Set `isLoading = true` in `OnInitializedAsync`

### 2. **Frontend/Pages/VehicleDamages.razor**
- ✅ Reordered UI checks (loading → auth → content)
- ✅ Set `isLoading = true` in `OnInitializedAsync`

### 3. **Frontend/Pages/ManageVehicles.razor**
- ✅ Reordered UI checks (loading → auth → content)
- ✅ Set `isLoading = true` in `OnInitializedAsync`

### 4. **Frontend/Pages/AdminDashboard.razor**
- ✅ Reordered UI checks (loading → auth → content)
- ✅ Set `isLoading = true` in `OnInitializedAsync`

### 5. **Frontend/Pages/Customers.razor**
- ✅ Reordered UI checks (loading → auth → content)
- ✅ Set `isLoading = true` in `OnInitializedAsync`

## Testing the Fix

### Before Fix:
1. Navigate to `/maintenances`
2. ❌ See "Access Denied" flash briefly
3. ✅ Content appears after flash

### After Fix:
1. Navigate to `/maintenances`
2. ✅ See loading spinner immediately
3. ✅ Content appears smoothly (no flash)

## Benefits

✅ **No more flickering** - Clean, professional UI  
✅ **Better UX** - Shows loading state instead of errors  
✅ **Consistent behavior** - All protected pages work the same  
✅ **Proper async handling** - Respects asynchronous auth checks  
✅ **Smooth transitions** - No jarring UI changes  

## Pattern to Follow

For any new protected pages, use this pattern:

```razor
@page "/your-page"

<PageTitle>Your Page</PageTitle>

<MudContainer MaxWidth="MaxWidth.ExtraExtraLarge">
    @if (isLoading)
    {
        <MudProgressLinear Indeterminate="true" Color="Color.Primary" />
    }
    else if (!isAuthorized)
    {
        <MudAlert Severity="Severity.Error" Variant="Variant.Filled">
            <strong>Access Denied</strong> - Admin or Employee privileges required.
        </MudAlert>
    }
    else
    {
        <!-- Your page content here -->
    }
</MudContainer>

@code {
    private bool isAuthorized = false;
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        isLoading = true;  // ← Always start with loading
        
        var role = await AuthService.GetRoleAsync();
        isAuthorized = role == "Admin" || role == "Employee";

        if (!isAuthorized)
        {
            isLoading = false;
            NavigationManager.NavigateTo("/");
            return;
        }

        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        try
        {
            // Load your data
        }
        catch (Exception ex)
        {
            // Handle errors
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

## Key Principles

1. **Always check loading first** in UI conditionals
2. **Initialize `isLoading = true`** at the start
3. **Set `isLoading = false`** on unauthorized access before returning
4. **Show loading spinner** while async operations run
5. **Show content only** when loading is complete AND authorized

## Status

✅ **All pages fixed**  
✅ **No compilation errors**  
✅ **Smooth user experience**  
✅ **Production ready**

The "Access Denied" flash is now completely eliminated! 🎉
