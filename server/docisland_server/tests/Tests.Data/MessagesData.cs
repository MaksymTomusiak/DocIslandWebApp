using Domain.Conversations;
using Domain.Messages;

namespace Tests.Data;

public static class MessagesData
{
    public static Message NewMessage(ConversationId conversationId) => Message.New("Hello world", conversationId);
}