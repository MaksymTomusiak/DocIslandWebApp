namespace Application.Conversations.Exceptions;

public class ConversationException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid Id { get; } = id;
}

public class ConversationNotFoundException(Guid id) 
    : ConversationException(id, $"Conversation under id: {id} not found!");

public class ConversationUserIdNotFoundException() 
    : ConversationException(Guid.Empty, $"User id not found!");

public class ConversationUserNotFoundException() 
    : ConversationException(Guid.Empty, $"Conversation user not found!");

public class ConversationFileNotFoundException() 
    : ConversationException(Guid.Empty, $"Conversation file not found!");

public class ConversationCantBeDeletedException() 
    : ConversationException(Guid.Empty, $"You can't delete this conversation!");

public class ConversationUnknownException(Guid id, Exception innerException)
    : ConversationException(id, $"Unknown exception for the Conversation under id: {id}!", innerException);