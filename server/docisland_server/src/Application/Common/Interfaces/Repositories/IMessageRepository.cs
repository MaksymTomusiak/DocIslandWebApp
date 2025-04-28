using Domain.Messages;

namespace Application.Common.Interfaces.Repositories;

public interface IMessageRepository
{
    Task<Message> Add(Message message, CancellationToken cancellationToken);
    Task<Message> Update(Message message, CancellationToken cancellationToken);
    Task<Message> Delete(Message message, CancellationToken cancellationToken);
}