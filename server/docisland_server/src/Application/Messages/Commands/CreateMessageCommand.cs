using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Conversations.Exceptions;
using Application.Messages.Exceptions;
using Domain.Conversations;
using Domain.Messages;
using LanguageExt;
using MediatR;

namespace Application.Messages.Commands;

public record CreateMessageCommand: IRequest<Either<MessageException, Message>>
{
    public required string Content { get; init; }
    public required Guid ConversationId { get; init; }
}

public class CreateMessageCommandHandler(IConversationQueries conversationQueries, IMessageRepository messageRepository): IRequestHandler<CreateMessageCommand, Either<MessageException, Message>>
{
    public async Task<Either<MessageException, Message>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var conversationId = new ConversationId(request.ConversationId);
        var existingConversation = await conversationQueries.GetById(conversationId, cancellationToken);

        //ToDo: check if user has access to this conversation
        return await existingConversation.Match(
            async m => await CreateMessage(request.Content, conversationId, cancellationToken),
            () => Task.FromResult<Either<MessageException, Message>>(new MessageConversationNotFoundException(request.ConversationId))
        );
    }
    
    private async Task<Either<MessageException,Message>> CreateMessage(string content, ConversationId conversationId,  CancellationToken cancellationToken)
    {
        try
        {
            var entity = Message.New(content, conversationId);
            
            return await messageRepository.Add(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            return new MessageUnknownException(Guid.Empty, ex);
        }
    }
}
