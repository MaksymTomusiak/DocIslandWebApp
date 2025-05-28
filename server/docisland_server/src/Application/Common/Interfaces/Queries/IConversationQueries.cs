using Domain.Conversations;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IConversationQueries
{
    Task<IReadOnlyList<Conversation>> GetAll(CancellationToken cancellationToken);
    Task<IReadOnlyList<Conversation>> GetRecentByUser(string userId, CancellationToken cancellationToken, int limit = 2);
    Task<IReadOnlyList<Conversation>> GetByUser(string userId, CancellationToken cancellationToken);
    Task<Option<Conversation>> GetById(ConversationId id, CancellationToken cancellationToken);
}