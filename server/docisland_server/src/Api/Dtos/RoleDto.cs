using Domain.Roles;

namespace Api.Dtos;

public record RoleDto(string Id, string Name, string Description)
{
    public static RoleDto FromDomainModel(Role role) 
        => new(role.Id, role.Name ?? string.Empty, role.Description);
}

public record CreateRoleDto(string Name, string Description);