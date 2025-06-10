using Application.Common.Interfaces.Queries;
using Application.Users.Exceptions;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record ToggleUserAdminCommand : IRequest<Either<UserException, bool>>
{
    public required string UserId { get; init; }
}

public class ToggleUserAdminCommandHandler : IRequestHandler<ToggleUserAdminCommand, Either<UserException, bool>>
{
    private readonly UserManager<User> _userManager;

    public ToggleUserAdminCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Either<UserException, bool>> Handle(ToggleUserAdminCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new UserNotFoundException(request.UserId);
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            return !isAdmin;
        }
        catch (Exception ex)
        {
            return new UserUnknownException(request.UserId, ex);
        }
    }
} 