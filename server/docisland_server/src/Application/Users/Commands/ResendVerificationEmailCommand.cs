using Application.Users.Exceptions;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands;

public record ResendVerificationEmailCommand : IRequest<Either<UserException, bool>>
{
    public required string Email { get; init; }
}

public class ResendVerificationEmailCommandHandler(
    UserManager<User> userManager) : IRequestHandler<ResendVerificationEmailCommand, Either<UserException, bool>>
{
    public async Task<Either<UserException, bool>> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new UserNotFoundException(Guid.Empty);
        }

        if (user.EmailConfirmed)
        {
            return true; // Or return a specific message indicating email is already verified
        }

        // Generate a new email verification token
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        user.EmailVerificationToken = token;
        user.EmailVerificationTokenExpiration = DateTime.UtcNow.AddHours(24); // Token expires in 24 hours

        await userManager.UpdateAsync(user);
        
        //ToDo: Add email sending
        return true;
    }
}