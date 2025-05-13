using Domain.Conversations;
using Domain.Files;
using LanguageExt;
using File = Domain.Files.File;

namespace Application.Common.Interfaces.Queries;

public interface IFileQueries
{
    Task<IReadOnlyList<File>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<File>> GetByUser(Guid userId, CancellationToken cancellationToken);
    Task<Option<File>> GetById(FileId id, CancellationToken cancellationToken);
    Task<Option<File>> GetByConversation(ConversationId conversationId, CancellationToken cancellationToken);
}