using Application.Messages.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class MessageErrorHandler
{
    public static ObjectResult ToObjectResult(this MessageException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                MessageNotFoundException
                    or MessageUserNotFoundException 
                    or MessageUserIdNotFoundException
                    or MessageConversationNotFoundException => StatusCodes.Status404NotFound,
                MessageCantBeDeletedException
                    or MessageUserWrongException => StatusCodes.Status403Forbidden,
                MessageUnknownException
                     or MessagLlmException=> StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Message error handler is not implemented")
            }
        };
    }
}