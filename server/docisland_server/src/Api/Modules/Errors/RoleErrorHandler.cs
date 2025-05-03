using Application.Roles.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class RoleErrorHandler
{
    public static ObjectResult ToObjectResult(this RoleException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                RoleNotFoundException or UserIdNotFoundException or UserNotFoundException => StatusCodes.Status404NotFound,
                RoleNameAlreadyExistsException => StatusCodes.Status409Conflict,
                RoleUnknownException => StatusCodes.Status500InternalServerError,
                UserUnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => throw new NotImplementedException("User error handler is not implemented")
            }
        };
    }
}