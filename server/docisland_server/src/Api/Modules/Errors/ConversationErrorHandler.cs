using Application.Conversations.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ConversationErrorHandler
{
    public static ObjectResult ToObjectResult(this ConversationException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                ConversationNotFoundException
                    or ConversationUserNotFoundException 
                    or ConversationUserIdNotFoundException
                    or ConversationFileNotFoundException => StatusCodes.Status404NotFound,
                ConversationCantBeDeletedException => StatusCodes.Status403Forbidden,
                ConversationUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Conversation error handler is not implemented")
            }
        };
    }
}