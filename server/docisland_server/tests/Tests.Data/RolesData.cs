using Domain.Roles;

namespace Tests.Data;

public static class RolesData
{
    public static Role AdminRole =>
        new Role("1","Admin", "Admin role");
    
    public static Role UserRole =>
        new Role("2","User", "User role");

    public static Role TestRole =>
        new("3","Test", "Test Role");
}