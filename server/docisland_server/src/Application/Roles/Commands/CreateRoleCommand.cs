using Application.Roles.Exceptions;
using Domain.Roles;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Roles.Commands;

public record CreateRoleCommand : IRequest<Either<RoleException, Role>>
{
    public required string Name { get; init; }
    public required string Description { get; init; }
}

public class CreateRoleCommandHandler(
    RoleManager<Role> roleManager) : IRequestHandler<CreateRoleCommand, Either<RoleException, Role>>
{
    public async Task<Either<RoleException, Role>> Handle(CreateRoleCommand request,
        CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByNameAsync(request.Name);
        if (role != null)
        {
            return new RoleNameAlreadyExistsException(role.Id, request.Name);
        }

        var newRole = new Role(Guid.NewGuid().ToString(), request.Name, request.Description);
        var result = await roleManager.CreateAsync(newRole);
        if (!result.Succeeded)
        {
            return new RoleUnknownException(String.Empty, new Exception("Could not create role!"));
        }
        return newRole;
    }
} 