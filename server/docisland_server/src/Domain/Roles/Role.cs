using Microsoft.AspNetCore.Identity;

namespace Domain.Roles;

public class Role(string name, string description) : IdentityRole<Guid>(name)
{
    public string Description { get; set; } = description;
}