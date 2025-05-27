using Application.Common.Interfaces.Queries;
using Application.Users.Exceptions;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record ToggleUserBanCommand : IRequest<Either<UserException, bool>>
{
    public required string UserId { get; init; }
}

public class ToggleUserBanCommandHandler : IRequestHandler<ToggleUserBanCommand, Either<UserException, bool>>
{
    private readonly UserManager<User> _userManager;

    public ToggleUserBanCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Either<UserException, bool>> Handle(ToggleUserBanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return new UserNotFoundException(request.UserId);
            }

            user.IsBanned = !user.IsBanned;
            await _userManager.UpdateAsync(user);

            return user.IsBanned;
        }
        catch (Exception ex)
        {
            return new UserUnknownException(request.UserId, ex);
        }
    }
} 