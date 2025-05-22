using System.Security.Claims;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Api.Middleware;

public class ClerkUserSyncMiddleware
{
    private readonly RequestDelegate _next;

    public ClerkUserSyncMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<User> userManager, ApplicationDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // Get claims from token
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? context.User.FindFirstValue("sub");
            
            var email = context.User.FindFirstValue(ClaimTypes.Email)
                        ?? context.User.FindFirstValue("email");
            
            var username = context.User.FindFirstValue("username");

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    // Create new user
                    user = new User
                    {
                        Id = userId,
                        Email = email ?? $"{userId}@clerk.user",
                        UserName = username ?? email ?? userId,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        await _next(context);
    }
}

public static class ClerkUserSyncMiddlewareExtensions
{
    public static IApplicationBuilder UseClerkUserSync(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ClerkUserSyncMiddleware>();
    }
} 