using System.Security.Claims;
using Application.Roles.Exceptions;
using Application.Users.Exceptions;
using Domain.Roles;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using UserIdNotFoundException = Application.Roles.Exceptions.UserIdNotFoundException;
using UserNotFoundException = Application.Roles.Exceptions.UserNotFoundException;
using UserUnauthorizedAccessException = Application.Roles.Exceptions.UserUnauthorizedAccessException;

namespace Application.Roles.Commands;

public record DeleteRoleCommand: IRequest<Either<RoleException, string>>
{
    public required Guid RoleId { get; init; }
}

public class DeleteRoleCommandHandler(
    RoleManager<Role> roleManager) : IRequestHandler<DeleteRoleCommand, Either<RoleException, string>>
{
    public async Task<Either<RoleException, string>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        
        var roleToDelete = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (roleToDelete == null)
        {
            return new RoleNotFoundException(request.RoleId);
        }

        try
        {
            var result = await roleManager.DeleteAsync(roleToDelete);
            return !result.Succeeded ? "Could not delete role" : "Role deleted successfully.";
        }
        catch (Exception ex)
        {
            return new RoleUnknownException(roleToDelete.Id, ex);
        }
    }
}