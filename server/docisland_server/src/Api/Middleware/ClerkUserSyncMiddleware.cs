using System.Security.Claims;
using Application.Common.Interfaces.Services.Synchronization;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Middleware;

public class ClerkUserSyncMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ClerkUserSyncMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
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
                using var scope = _serviceScopeFactory.CreateScope();
                var userSyncService = scope.ServiceProvider.GetRequiredService<IUserSyncService>();
                await userSyncService.SyncUserAsync(userId, email, username);
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