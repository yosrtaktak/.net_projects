using Microsoft.AspNetCore.Identity;
using Backend.Core.Entities;

namespace Backend.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Créer les rôles
        string[] roleNames = { "Admin", "Employee", "Customer" };
        
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Créer l'utilisateur admin
        var adminUser = await userManager.FindByNameAsync("admin");
        
        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@carrental.com",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // Créer un utilisateur employee de test
        var employeeUser = await userManager.FindByNameAsync("employee");
        
        if (employeeUser == null)
        {
            var employee = new ApplicationUser
            {
                UserName = "employee",
                Email = "employee@carrental.com",
                EmailConfirmed = true,
                FirstName = "Employee",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(employee, "Employee@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(employee, "Employee");
            }
        }

        // Créer un utilisateur customer de test
        var customerUser = await userManager.FindByNameAsync("customer");
        
        if (customerUser == null)
        {
            var customer = new ApplicationUser
            {
                UserName = "customer",
                Email = "customer@carrental.com",
                EmailConfirmed = true,
                FirstName = "Customer",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(customer, "Customer@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(customer, "Customer");
            }
        }
    }
}
