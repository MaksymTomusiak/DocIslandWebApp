using Domain.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitializer(ApplicationDbContext context, RoleManager<Role> roleManager, IConfiguration configuration)
{
    private readonly bool _isTesting = configuration.GetValue<bool>("IsTesting");
    public async Task InitializeAsync()
    {
        await context.Database.MigrateAsync();
        if (!_isTesting)
        {
            await Seeders.RoleSeeder.SeedRolesAsync(roleManager);
        }
    }
}