using Domain.Conversations;

namespace Api.Dtos;

public record ConversationDto(
    Guid Id,
    string UserId,
    string FileName,
    DateTime CreatedAt)
{
    public static ConversationDto FromDomainModel(Conversation conversation) => new (
        conversation.Id.Value,
        conversation.UserId,
        conversation.File!.OriginalFileName,
        conversation.CreatedAt);
}

public record ConversationCreateDto(IFormFile File);