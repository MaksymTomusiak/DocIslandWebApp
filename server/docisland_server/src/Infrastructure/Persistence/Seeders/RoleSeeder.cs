using Domain.Roles;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence.Seeders;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(RoleManager<Role> roleManager)
    {
        var roles = new List<Role> {
                new("1", "Admin", "Admin role"),
                new("2", "User", "Default user role")
            };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }
}