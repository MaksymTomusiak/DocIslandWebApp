using Domain.Conversations;
using Domain.Messages;

namespace Api.Dtos;

public record MessageDto(
    Guid Id,
    string Content,
    Guid ConversationId,
    DateTime CreatedAt,
    bool IsResponse)
{
    public static MessageDto FromDomainModel(Message message) => new(
        message.Id.Value, 
        message.Content, 
        message.ConversationId.Value,
        message.CreatedAt,
        message.IsResponse);
}

public record MessageCreateDto(Guid ConversationId, string Content);