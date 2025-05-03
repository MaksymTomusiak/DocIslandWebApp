using Domain.Conversations;
using Domain.Messages;

namespace Api.Dtos;

public record MessageDto(
    Guid Id,
    string Content,
    Guid ConversationId)
{
    public static MessageDto FromDomainModel(Message message) => new(
        message.Id.Value, 
        message.Content, 
        message.ConversationId.Value);
}

public record MessageCreateDto(Guid ConversationId, string Content);