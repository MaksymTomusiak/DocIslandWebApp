using System.Security.Claims;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Middleware;

public class ClerkUserSyncMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private const string UserCacheKeyPrefix = "user_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public ClerkUserSyncMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<User> userManager, ApplicationDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? context.User.FindFirstValue("sub");
            
            var email = context.User.FindFirstValue(ClaimTypes.Email)
                        ?? context.User.FindFirstValue("email");
            
            var username = context.User.FindFirstValue("username");

            if (!string.IsNullOrEmpty(userId))
            {
                var cacheKey = $"{UserCacheKeyPrefix}{userId}";
                var user = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                    return await userManager.FindByIdAsync(userId);
                });

                if (user == null)
                {
                    // Check if this is the first user in the system
                    var isFirstUser = !await userManager.Users.AnyAsync();

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

                    if (isFirstUser)
                    {
                        // Make the first user an admin
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        // Add default role for other users
                        await userManager.AddToRoleAsync(user, "User");
                    }

                    // Cache the newly created user
                    _cache.Set(cacheKey, user, CacheDuration);
                }
                else
                {
                    // Check if user data has changed
                    var needsUpdate = false;
                    var updatedUser = new User
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        EmailConfirmed = user.EmailConfirmed,
                        IsBanned = user.IsBanned
                    };

                    if (email != null && email != user.Email)
                    {
                        updatedUser.Email = email;
                        needsUpdate = true;
                    }

                    if (username != null && username != user.UserName)
                    {
                        updatedUser.UserName = username;
                        needsUpdate = true;
                    }

                    if (needsUpdate)
                    {
                        // Update user properties
                        user.Email = updatedUser.Email;
                        user.UserName = updatedUser.UserName;

                        var updateResult = await userManager.UpdateAsync(user);
                        if (!updateResult.Succeeded)
                        {
                            throw new Exception($"Failed to update user: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                        }

                        // Update cache
                        _cache.Set(cacheKey, user, CacheDuration);
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