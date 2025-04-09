using Domain.Conversations;

namespace Application.Common.Interfaces.Repositories;

public interface IConversationRepository
{
    Task<Conversation> Add(Conversation conversation, CancellationToken cancellationToken);
    Task<Conversation> Update(Conversation conversation, CancellationToken cancellationToken);
    Task<Conversation> Delete(Conversation conversation, CancellationToken cancellationToken);
}