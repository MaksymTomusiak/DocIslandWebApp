using Domain.Users;

namespace Application.Messages.Exceptions;

public class MessageException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class MessageNotFoundException(Guid id)
    : MessageException(id, $"Message under id: {id} not found!");

public class MessageUserIdNotFoundException(Guid id)
    : MessageException(Guid.Empty, $"User with id: {id} not found!");

public class MessageUserWrongException()
    : MessageException(Guid.Empty, $"You can't create message for another user!");

public class MessageUserNotFoundException(User user)
    : MessageException(Guid.Empty, $"User {user.UserName} not found!");

public class MessageCantBeDeletedException()
    : MessageException(Guid.Empty, $"You can't delete this message!");

public class MessagLlmException(Exception innerException)
    : MessageException(Guid.Empty, $"Error while creating message!", innerException);

public class MessageUnknownException(Guid id, Exception innerException)
    : MessageException(id, $"Unknown exception for the Message under id: {id}!", innerException);
    
public class MessageConversationNotFoundException(Guid id)
    : MessageException(id, $"Conversation under id: {id} not found!");