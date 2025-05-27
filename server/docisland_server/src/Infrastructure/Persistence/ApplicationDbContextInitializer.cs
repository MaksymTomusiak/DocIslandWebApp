using Domain.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitializer(ApplicationDbContext context, RoleManager<Role> roleManager)
{
    public async Task InitializeAsync()
    {
        await context.Database.MigrateAsync();
        await Seeders.RoleSeeder.SeedRolesAsync(roleManager);
    }
}