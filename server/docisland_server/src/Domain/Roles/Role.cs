using Microsoft.AspNetCore.Identity;

namespace Domain.Roles;

public class Role : IdentityRole<Guid>
{
    public string Description { get; set; }
    private Role(string name, string description) : base(name) => Description = description;
}