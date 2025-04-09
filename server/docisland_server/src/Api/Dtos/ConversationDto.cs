using Domain.Conversations;

namespace Api.Dtos;

public record ConversationDto(
    Guid Id,
    Guid UserId,
    Guid FileId,
    DateTime CreatedAt)
{
    public static ConversationDto FromDomainModel(Conversation conversation) => new (
        conversation.Id.Value,
        conversation.UserId,
        conversation.FileId.Value,
        conversation.CreatedAt);
}

public record ConversationCreateDto(IFormFile File);