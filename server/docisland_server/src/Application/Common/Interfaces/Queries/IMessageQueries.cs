using Domain.Messages;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IMessageQueries
{
    Task<IReadOnlyList<Message>> GetAll(CancellationToken cancellationToken);
    Task<Option<Message>> GetById(MessageId id, CancellationToken cancellationToken);
}