# Fix: Registration Failing with 400 Bad Request

## Problem
Registration is failing with this request:
```json
{
  "username": "testt",
  "email": "test@ggg.ccc",
  "password": "123456Tt@",
  "role": null
}
```

Error: `POST https://localhost:5000/api/auth/register 400 (Bad Request)`

## Root Cause Analysis

The password `123456Tt@` meets all Identity requirements:
- ? Has digit: `6`
- ? Has lowercase: `t`
- ? Has uppercase: `T`
- ? Has special char: `@`
- ? Length: 9 characters (min 6 required)

The likely issue is **the "Customer" role doesn't exist** in the database yet.

## Check If Roles Exist

Run this SQL query:
```sql
USE CarRentalDB;

SELECT * FROM AspNetRoles;
```

**Expected roles**:
- Admin
- Employee
- Customer

If the Customer role is missing, the registration will fail.

## Solution 1: Quick Fix - Register Without Role Check

Update `AuthController.cs` to handle missing roles gracefully:

```csharp
[HttpPost("register")]
public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
{
    // Vérifier si l'utilisateur existe déjà
    var existingUser = await _userManager.FindByNameAsync(dto.Username);
    if (existingUser != null)
    {
        return BadRequest(new { message = "Username already exists" });
    }

    var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
    if (existingEmail != null)
    {
        return BadRequest(new { message = "Email already exists" });
    }

    // Créer le nouvel utilisateur avec les champs Customer intégrés
    var user = new ApplicationUser
    {
        UserName = dto.Username,
        Email = dto.Email,
        EmailConfirmed = true,
        CreatedAt = DateTime.UtcNow,
        FirstName = dto.Username, // Can be updated later in profile
        LastName = "", // Can be updated later in profile
        RegistrationDate = DateTime.UtcNow,
        Tier = CustomerTier.Standard
    };

    var result = await _userManager.CreateAsync(user, dto.Password);

    if (!result.Succeeded)
    {
        return BadRequest(new { 
            message = "User creation failed", 
            errors = result.Errors.Select(e => e.Description) 
        });
    }

    // Assigner le rôle (par défaut Customer)
    var roleToAssign = string.IsNullOrEmpty(dto.Role) ? "Customer" : dto.Role;
    
    // Check if role exists before assigning
    var roleExists = await _roleManager.RoleExistsAsync(roleToAssign);
    if (roleExists)
    {
        await _userManager.AddToRoleAsync(user, roleToAssign);
    }
    else
    {
        // Create the role if it doesn't exist
        await _roleManager.CreateAsync(new IdentityRole(roleToAssign));
        await _userManager.AddToRoleAsync(user, roleToAssign);
    }

    // Générer le token
    var roles = await _userManager.GetRolesAsync(user);
    var token = _jwtService.GenerateToken(user, roles);

    return Ok(new AuthResponseDto
    {
        Token = token,
        Username = user.UserName!,
        Email = user.Email!,
        Role = roles.FirstOrDefault() ?? "Customer"
    });
}
```

## Solution 2: Ensure Roles Are Created at Startup

Check if `DbInitializer.InitializeAsync` is creating the roles properly. It should be in your code already.

Update `Backend/Infrastructure/Data/DbInitializer.cs` (if needed):

```csharp
public static class DbInitializer
{
    public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        string[] roleNames = { "Admin", "Employee", "Customer" };
        
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                Console.WriteLine($"? Created role: {roleName}");
            }
        }

        // Create admin user if doesn't exist
        var adminEmail = "admin@carrental.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                CreatedAt = DateTime.UtcNow,
                RegistrationDate = DateTime.UtcNow,
                Tier = CustomerTier.Platinum
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("? Created admin user");
            }
        }

        // Create employee user if doesn't exist
        var employeeEmail = "employee@carrental.com";
        var employeeUser = await userManager.FindByEmailAsync(employeeEmail);
        
        if (employeeUser == null)
        {
            employeeUser = new ApplicationUser
            {
                UserName = "employee",
                Email = employeeEmail,
                EmailConfirmed = true,
                FirstName = "Employee",
                LastName = "User",
                CreatedAt = DateTime.UtcNow,
                RegistrationDate = DateTime.UtcNow,
                Tier = CustomerTier.Standard
            };

            var result = await userManager.CreateAsync(employeeUser, "Employee@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(employeeUser, "Employee");
                Console.WriteLine("? Created employee user");
            }
        }

        // Create test customer if doesn't exist
        var customerEmail = "customer@carrental.com";
        var customerUser = await userManager.FindByEmailAsync(customerEmail);
        
        if (customerUser == null)
        {
            customerUser = new ApplicationUser
            {
                UserName = "customer",
                Email = customerEmail,
                EmailConfirmed = true,
                FirstName = "Test",
                LastName = "Customer",
                CreatedAt = DateTime.UtcNow,
                RegistrationDate = DateTime.UtcNow,
                Tier = CustomerTier.Gold
            };

            var result = await userManager.CreateAsync(customerUser, "Customer@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(customerUser, "Customer");
                Console.WriteLine("? Created customer user");
            }
        }
    }
}
```

## Quick Test

### Option A: Use Test Account
Instead of registering, login with an existing test account:
```json
{
  "username": "customer",
  "password": "Customer@123"
}
```

### Option B: Create Roles Manually in SQL
```sql
USE CarRentalDB;

-- Check if roles exist
SELECT * FROM AspNetRoles;

-- If Customer role is missing, add it:
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES 
    (NEWID(), 'Customer', 'CUSTOMER', NEWID());

-- Verify
SELECT * FROM AspNetRoles;
```

### Option C: Restart Backend
1. **Stop** the backend (if running)
2. **Start** it again - this will trigger `DbInitializer.InitializeAsync()`
3. Check console output for "? Created role: Customer"

## Verify the Fix

After implementing the solution, test registration:

### Request:
```bash
curl -X POST https://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testt",
    "email": "test@ggg.ccc",
    "password": "123456Tt@",
    "role": null
  }'
```

### Expected Success Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "testt",
  "email": "test@ggg.ccc",
  "role": "Customer"
}
```

### Expected Error Response (if still failing):
```json
{
  "message": "User creation failed",
  "errors": [
    "Passwords must have at least one non alphanumeric character.",
    "Passwords must have at least one lowercase ('a'-'z').",
    etc...
  ]
}
```

## Common Registration Errors

| Error | Cause | Solution |
|-------|-------|----------|
| "Username already exists" | Username "testt" is taken | Use different username |
| "Email already exists" | Email is already registered | Use different email |
| "User creation failed" + password errors | Password doesn't meet requirements | Use stronger password |
| 400 without specific message | Role doesn't exist | Implement Solution 1 or 2 above |
| 500 Internal Server Error | Database connection issue | Check connection string |

## Debug Steps

1. **Check backend console** for error messages when registration is attempted
2. **Check browser Network tab** (F12 ? Network) for the actual error response body
3. **Check AspNetRoles table** to confirm roles exist:
   ```sql
   SELECT * FROM AspNetRoles;
   ```
4. **Check AspNetUsers table** to see if user was partially created:
   ```sql
   SELECT * FROM AspNetUsers WHERE UserName = 'testt';
   ```

## Final Notes

The most likely issue is that the **"Customer" role hasn't been created yet**. Restarting the backend should fix this if the `DbInitializer` is set up properly in `Program.cs`.

If the problem persists, check the browser console for the actual error message in the response body.
