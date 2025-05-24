using System.Security.Claims;

namespace Api.Extensions;

public static class HttpContextUserExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? throw new UnauthorizedAccessException("User ID not found.");
    }
}