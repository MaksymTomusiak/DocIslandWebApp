using System.Security.Claims;
using Application.Users.Exceptions;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record UpdateUserPasswordCommand : IRequest<Either<UserException, User>>
{
    public required string OldPassword { get; init; }
    public required string NewPassword { get; init; }
}

public class UpdateUserPasswordCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager) : IRequestHandler<UpdateUserPasswordCommand, Either<UserException, User>>
{
    public async Task<Either<UserException, User>> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var sessionUserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(sessionUserId))
        {
            return new UserIdNotFoundException();
        }

        var sessionUser = await userManager.FindByIdAsync(sessionUserId);
        if (sessionUser == null)
        {
            return new UserNotFoundException(new Guid(sessionUserId));
        }

        var result = await userManager.ChangePasswordAsync(sessionUser, request.OldPassword, request.NewPassword);
        
        if (!result.Succeeded)
        {
            return new InvalidCredentialsException();
        }
        
        await userManager.UpdateAsync(sessionUser);
        return sessionUser;
    }
}