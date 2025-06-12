using System.Security.Claims;
using Application.Common.Interfaces.Services.Synchronization;

namespace Api.Middleware;

public class ClerkUserSyncMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IUserSyncService _userSyncService;

    public ClerkUserSyncMiddleware(RequestDelegate next, IUserSyncService userSyncService)
    {
        _next = next;
        _userSyncService = userSyncService;
    }

    public async Task InvokeAsync(HttpContext context)
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
                await _userSyncService.SyncUserAsync(userId, email, username);
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