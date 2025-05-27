using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Domain.Users;

namespace Api.Authorization;

public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
{
    private readonly UserManager<User> _userManager;

    public AdminAuthorizationHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRequirement requirement)
    {
        var user = await _userManager.GetUserAsync(context.User);
        if (user == null)
        {
            return;
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            context.Succeed(requirement);
        }
    }
}

public class AdminRequirement : IAuthorizationRequirement { } 