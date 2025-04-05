using Domain.Conversations;

namespace Domain.Messages;

public class Message
{
    public MessageId Id { get; private set; }
    public string Content { get; set; }
    public ConversationId ConversationId { get; set; }
    public Conversation? Conversation { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsResponse { get; set; }
    
    private Message(MessageId id, string content, ConversationId conversationId, DateTime createdAt, bool isResponse) 
    {
        Id = id;
        Content = content;
        ConversationId = conversationId;
        CreatedAt = createdAt;
        IsResponse = isResponse;
    }

    public static Message New(string content, ConversationId conversationId, bool isResponse) =>
        new Message(MessageId.New(), content, conversationId, DateTime.UtcNow, isResponse);

    public void UpdateContent(string content) => Content = content;
}