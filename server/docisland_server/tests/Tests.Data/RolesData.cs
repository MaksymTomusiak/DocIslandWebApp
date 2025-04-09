using Domain.Roles;

namespace Tests.Data;

public static class RolesData
{
    public static Role AdminRole =>
        new Role("Admin", "Admin role");
    
    public static Role UserRole =>
        new Role("User", "User role");
}