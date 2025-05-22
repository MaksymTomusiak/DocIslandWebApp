using System.Security.Claims;
using Application.Users.Exceptions;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record UpdateUserNameCommand : IRequest<Either<UserException, User>>
{
    public required string UserName { get; init; }
}

public class UpdateUserNameCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager) : IRequestHandler<UpdateUserNameCommand, Either<UserException, User>>
{
    public async Task<Either<UserException, User>> Handle(UpdateUserNameCommand request, CancellationToken cancellationToken)
    {
        var sessionUserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(sessionUserId))
        {
            return new UserIdNotFoundException();
        }

        var sessionUser = await userManager.FindByIdAsync(sessionUserId);
        if (sessionUser == null)
        {
            return new UserNotFoundException(sessionUserId);
        }

        var existingUser = await userManager.FindByNameAsync(request.UserName);

        if (existingUser != null)
        {
            return new UserWithNameAlreadyExistsException(sessionUserId);
        }
        
        sessionUser.UserName = request.UserName;
        await userManager.UpdateAsync(sessionUser);
        
        return sessionUser;
    }
}