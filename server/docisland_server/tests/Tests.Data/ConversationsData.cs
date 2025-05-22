using Domain.Conversations;
using Domain.Files;

namespace Tests.Data;

public class ConversationsData
{
    public static Conversation NewConversation(string userId, FileId fileId) =>
        Conversation.New(userId, fileId);
}