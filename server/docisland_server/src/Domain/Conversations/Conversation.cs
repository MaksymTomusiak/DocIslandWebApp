using Domain.Files;
using Domain.Messages;
using Domain.Users;
using File = Domain.Files.File;

namespace Domain.Conversations;

public class Conversation
{
    public ConversationId Id { get; private set; }
    public string UserId { get; private set; }
    public User? User { get; private set; }
    public FileId FileId { get; private set; }
    public File? File { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ICollection<Message> Messages = new List<Message>();
    
    private Conversation(ConversationId id, string userId, FileId fileId, DateTime createdAt) 
    {
        Id = id;
        UserId = userId;
        FileId = fileId;
        CreatedAt = createdAt;
    }

    public static Conversation New(string userId, FileId fileId) =>
        new Conversation(ConversationId.New(), userId, fileId, DateTime.UtcNow);
}