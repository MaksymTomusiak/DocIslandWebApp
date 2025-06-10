using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Messages.Exceptions;
using Domain.Messages;
using LanguageExt;
using MediatR;

namespace Application.Messages.Commands;

public record DeleteMessageCommandValidator : IRequest<Either<MessageException, Message>>
{
    public required Guid MessageId { get; init; }
}

public class DeleteMessageCommandValidatorHandler(IMessageQueries messageQueries, IMessageRepository messageRepository)
    : IRequestHandler<DeleteMessageCommandValidator, Either<MessageException, Message>>
{
    public async Task<Either<MessageException, Message>> Handle(DeleteMessageCommandValidator request,
        CancellationToken cancellationToken)
    {
        //todo: check if user has access to this conversation
        
        var messageId = new MessageId(request.MessageId);
        var existingMessage = await messageQueries.GetById(messageId, cancellationToken);
        return await existingMessage.Match(
            async m =>
            {
                return await DeleteMesage(m, cancellationToken);
            },
            () => Task.FromResult<Either<MessageException, Message>>(new MessageNotFoundException(request.MessageId)));
    }

    private async Task<Either<MessageException, Message>> DeleteMesage(Message message, CancellationToken cancellationToken)
    {
        try
        {
            //ToDo: Add file deleting
            
            return await messageRepository.Delete(message, cancellationToken);
        }
        catch (Exception ex)
        {
            return new MessageUnknownException(Guid.Empty, ex);
        }
    }
}