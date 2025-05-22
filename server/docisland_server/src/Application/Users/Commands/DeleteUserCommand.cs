using System.Security.Claims;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services.Files;
using Application.Users.Exceptions;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record DeleteUserCommand : IRequest<Either<UserException, string>>
{
    public required string UserId { get; init; }
}

public class DeleteUserCommandHandler(
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor,
    IFileQueries fileQueries,
    IFileRepository fileRepository,
    IFileStorageService fileStorageService) : IRequestHandler<DeleteUserCommand, Either<UserException, string>>
{
    public async Task<Either<UserException, string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var sessionUserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(sessionUserId))
        {
            return new UserIdNotFoundException();
        }

        var userToDelete = await userManager.FindByIdAsync(request.UserId.ToString());
        if (userToDelete == null)
        {
            return new UserNotFoundException(request.UserId);
        }

        var sessionUserIsAdmin = (bool)httpContextAccessor.HttpContext?.User.IsInRole("Admin");
        var targetUserIsAdmin = await userManager.IsInRoleAsync(userToDelete, "Admin");

        // A normal user can only delete their own account
        if (!sessionUserIsAdmin &&sessionUserId != userToDelete.Id)
        {
            return new UserUnauthorizedAccessException("You can only delete your own account.");
        }

        // An admin can delete themselves or normal users, but not another admin
        if (sessionUserIsAdmin && targetUserIsAdmin && sessionUserId != userToDelete.Id)
        {
            return new UserUnauthorizedAccessException("Admins cannot delete other admin accounts.");
        }

        try
        {
            var result = await userManager.DeleteAsync(userToDelete);
            const string conversationsFiles = "conversations-files";
            var files = await fileQueries.GetByUser(userToDelete.Id, cancellationToken);
            foreach (var file in files)
            {
                await fileRepository.Delete(file, cancellationToken);
                await fileStorageService.DeleteFileAsync(conversationsFiles, file.Id.Value, cancellationToken);
            }
            
            return !result.Succeeded ? "Could not delete user" : "User account deleted successfully.";
        }
        catch (Exception ex)
        {
            return new UserUnknownException(userToDelete.Id, ex);
        }
    }
}