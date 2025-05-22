using Domain.Conversations;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IConversationQueries
{
    Task<IReadOnlyList<Conversation>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<Conversation>> GetByUser(string userId, CancellationToken cancellationToken);
    Task<Option<Conversation>> GetById(ConversationId id, CancellationToken cancellationToken);
}