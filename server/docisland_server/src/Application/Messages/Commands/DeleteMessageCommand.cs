using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Messages.Exceptions;
using Domain.Messages;
using LanguageExt;
using MediatR;

namespace Application.Messages.Commands;

public record DeleteMessageCommand : IRequest<Either<MessageException, Message>>
{
    public required Guid MessageId { get; init; }
}

public class DeleteMessageCommandHandler(
    IMessageQueries messageQueries,
    IMessageRepository messageRepository) : IRequestHandler<DeleteMessageCommand, Either<MessageException, Message>>
{
    public async Task<Either<MessageException, Message>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var messageId = new MessageId(request.MessageId);
        
        var existingMessage = await messageQueries.GetById(messageId, cancellationToken);
        return await existingMessage.Match(
            async m => await DeleteMessage(m, cancellationToken),
            () => Task.FromResult<Either<MessageException, Message>>(new MessageNotFoundException(request.MessageId))
        );
    }

    private async Task<Either<MessageException, Message>> DeleteMessage(Message message, CancellationToken cancellationToken)
    {
        await messageRepository.Delete(message, cancellationToken);
        return message;
    }
}