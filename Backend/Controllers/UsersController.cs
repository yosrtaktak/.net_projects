using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Backend.Core.Entities;
using Backend.Application.DTOs;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: api/users/me
    // Get current authenticated user's profile
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMyProfile()
    {
        // Try to get email from claims
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value 
            ?? User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.Identity?.Name;
            
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized(new { message = "User not authenticated - no email claim found" });
        }

        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return NotFound(new { message = $"User not found for email: {userEmail}" });
        }

        var roles = await _userManager.GetRolesAsync(user);

        var profile = new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            DriverLicenseNumber = user.DriverLicenseNumber ?? "",
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            RegistrationDate = user.RegistrationDate,
            Tier = user.Tier,
            UserName = user.UserName,
            Roles = roles
        };

        return Ok(profile);
    }

    // GET: api/users/me/rentals
    // Get current user's rental history
    [HttpGet("me/rentals")]
    public async Task<ActionResult<IEnumerable<Rental>>> GetMyRentals()
    {
        // Try to get email from claims
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value 
            ?? User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.Identity?.Name;
            
        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized(new { message = "User not authenticated" });
        }

        var user = await _userManager.Users
            .Include(u => u.Rentals)
                .ThenInclude(r => r.Vehicle)
            .Include(u => u.Rentals)
                .ThenInclude(r => r.Payment)
            .FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var rentals = user.Rentals
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        return Ok(rentals);
    }

    // PUT: api/users/me
    // Update current user's profile
    [HttpPut("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateMyProfile([FromBody] UpdateProfileDto dto)
    {
        try
        {
            // Try to get email from claims
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value 
                ?? User.FindFirst(ClaimTypes.Name)?.Value
                ?? User.Identity?.Name;
                
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { message = "User not authenticated" });
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Update fields with null checks
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.DriverLicenseNumber = dto.DriverLicenseNumber ?? user.DriverLicenseNumber;
            
            // Only update DateOfBirth if provided
            if (dto.DateOfBirth.HasValue)
            {
                user.DateOfBirth = dto.DateOfBirth;
            }
            
            // Only update Address if not null
            if (dto.Address != null)
            {
                user.Address = dto.Address;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = "Failed to update profile", errors = errors });
            }

            // Return updated profile
            return await GetMyProfile();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating profile", error = ex.Message });
        }
    }

    // GET: api/users/customers
    // Admin/Employee only: Get all users with Customer role
    [Authorize(Roles = "Admin,Employee")]
    [HttpGet("customers")]
    public async Task<ActionResult<IEnumerable<CustomerListDto>>> GetAllCustomers()
    {
        var allUsers = await _userManager.Users
            .Include(u => u.Rentals)
            .ToListAsync();

        var customers = new List<CustomerListDto>();

        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Customer"))
            {
                customers.Add(new CustomerListDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    Tier = user.Tier,
                    RegistrationDate = user.RegistrationDate,
                    TotalRentals = user.Rentals.Count
                });
            }
        }

        return Ok(customers.OrderByDescending(c => c.RegistrationDate));
    }

    // GET: api/users/{id}
    // Get specific user profile (admin/employee or own profile)
    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // If customer role, only allow viewing own profile
        if (User.IsInRole("Customer"))
        {
            var currentUserEmail = User.Identity?.Name;
            if (user.Email != currentUserEmail)
            {
                return Forbid();
            }
        }

        var roles = await _userManager.GetRolesAsync(user);

        var profile = new UserProfileDto
        {
            Id = user.Id,
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            DriverLicenseNumber = user.DriverLicenseNumber ?? "",
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            RegistrationDate = user.RegistrationDate,
            Tier = user.Tier,
            UserName = user.UserName,
            Roles = roles
        };

        return Ok(profile);
    }

    // PUT: api/users/{id}
    // Admin/Employee: Update any user's profile
    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}")]
    public async Task<ActionResult<UserProfileDto>> UpdateUserProfile(string id, [FromBody] UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;
        user.DriverLicenseNumber = dto.DriverLicenseNumber;
        user.DateOfBirth = dto.DateOfBirth;
        user.Address = dto.Address;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Failed to update profile", errors = result.Errors });
        }

        return await GetUserProfile(id);
    }

    // PUT: api/users/{id}/customer
    // Admin only: Update customer with tier
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/customer")]
    public async Task<ActionResult<UserProfileDto>> AdminUpdateCustomer(string id, [FromBody] AdminUpdateCustomerDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;
        user.DriverLicenseNumber = dto.DriverLicenseNumber;
        user.DateOfBirth = dto.DateOfBirth;
        user.Address = dto.Address;
        user.Tier = dto.Tier;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Failed to update customer", errors = result.Errors });
        }

        return await GetUserProfile(id);
    }

    // PUT: api/users/{id}/tier
    // Admin only: Update user's tier
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/tier")]
    public async Task<ActionResult<UserProfileDto>> UpdateUserTier(string id, [FromBody] UpdateTierDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        user.Tier = dto.Tier;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Failed to update tier", errors = result.Errors });
        }

        return await GetUserProfile(id);
    }

    // GET: api/users/employees
    // Admin only: Get all employees
    [Authorize(Roles = "Admin")]
    [HttpGet("employees")]
    public async Task<ActionResult<IEnumerable<EmployeeListDto>>> GetAllEmployees()
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var employees = new List<EmployeeListDto>();

        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin") || roles.Contains("Employee"))
            {
                employees.Add(new EmployeeListDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    RegistrationDate = user.RegistrationDate,
                    LastLogin = user.LastLogin,
                    Roles = roles
                });
            }
        }

        return Ok(employees.OrderByDescending(e => e.RegistrationDate));
    }

    // POST: api/users/employees
    // Admin only: Create new employee
    [Authorize(Roles = "Admin")]
    [HttpPost("employees")]
    public async Task<ActionResult<UserProfileDto>> CreateEmployee([FromBody] CreateEmployeeDto dto)
    {
        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Validate role
        if (dto.Role != "Admin" && dto.Role != "Employee")
        {
            return BadRequest(new { message = "Role must be either 'Admin' or 'Employee'" });
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            RegistrationDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Failed to create employee", errors = result.Errors });
        }

        // Assign role
        var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
        if (!roleResult.Succeeded)
        {
            // Rollback user creation
            await _userManager.DeleteAsync(user);
            return BadRequest(new { message = "Failed to assign role", errors = roleResult.Errors });
        }

        return await GetUserProfile(user.Id);
    }

    // PUT: api/users/{id}/role
    // Admin only: Update user's role
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/role")]
    public async Task<ActionResult<UserProfileDto>> UpdateUserRole(string id, [FromBody] UpdateUserRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Validate role
        if (dto.Role != "Admin" && dto.Role != "Employee" && dto.Role != "Customer")
        {
            return BadRequest(new { message = "Invalid role. Must be 'Admin', 'Employee', or 'Customer'" });
        }

        // Get current roles
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Remove all current roles
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest(new { message = "Failed to remove current roles", errors = removeResult.Errors });
            }
        }

        // Add new role
        var addResult = await _userManager.AddToRoleAsync(user, dto.Role);
        if (!addResult.Succeeded)
        {
            return BadRequest(new { message = "Failed to add new role", errors = addResult.Errors });
        }

        return await GetUserProfile(id);
    }

    // DELETE: api/users/{id}
    // Admin only: Delete user (soft delete or hard delete)
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Check if user has active rentals
        var hasActiveRentals = user.Rentals.Any(r => r.Status == RentalStatus.Active || r.Status == RentalStatus.Reserved);
        if (hasActiveRentals)
        {
            return BadRequest(new { message = "Cannot delete user with active or reserved rentals" });
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Failed to delete user", errors = result.Errors });
        }

        return NoContent();
    }
}
