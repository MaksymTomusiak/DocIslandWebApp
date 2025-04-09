using Application.Common.Interfaces;
using Application.Common.Interfaces.Services.Jwt;
using Application.Users.Exceptions;
using Domain.Roles;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

public record LoginUserCommand : IRequest<Either<UserException, string>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginUserCommandHandler(
    IJwtProvider jwtProvider,
    UserManager<User> userManager,
    RoleManager<Role> roleManager) : IRequestHandler<LoginUserCommand, Either<UserException, string>>
{
    private const string UserRoleName = "User";

    public async Task<Either<UserException, string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new InvalidCredentialsException();
        }

        // Check if the user's email is confirmed
        if (!user.EmailConfirmed)
        {
            return new EmailNotVerifiedException(user.Id);
        }

        // Verify password
        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            return new InvalidCredentialsException();
        }

        var roles = await userManager.GetRolesAsync(user);
        
        if (roles.Count == 0)
        {
            var existingUserRole = await roleManager.FindByNameAsync(UserRoleName);
            if (existingUserRole == null)
            {
                try
                {
                    await roleManager.CreateAsync(new Role(UserRoleName, "Default user role"));
                }
                catch (Exception e)
                {
                    return new UserUnknownException(user.Id, e);
                }
            }
            
            await userManager.AddToRoleAsync(user, UserRoleName);
        }
        
        var roleName = roles.FirstOrDefault() ?? UserRoleName;
        var role = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken);

        if (role == null)
        {
            return new UserRoleNotFoundException();
        }
        
        return jwtProvider.Generate(user, role);
    }
}