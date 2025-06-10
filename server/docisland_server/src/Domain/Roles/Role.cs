using Microsoft.AspNetCore.Identity;

namespace Domain.Roles;

public class Role : IdentityRole<string>
{
    public string Description { get; init; }

    public Role(string id, string name, string description)
        : base(name)
    {
        Id = id;
        Description = description;
    }
}