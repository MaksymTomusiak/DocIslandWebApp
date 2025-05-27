using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

public class AdminAuthorizeAttribute : AuthorizeAttribute
{
    public AdminAuthorizeAttribute()
    {
        Policy = "AdminPolicy";
    }
} 