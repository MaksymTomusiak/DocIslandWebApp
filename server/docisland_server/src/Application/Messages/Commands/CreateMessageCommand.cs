using System.Security.Claims;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services.LLM;
using Application.Messages.Exceptions;
using Domain.Conversations;
using Domain.Messages;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Messages.Commands;

public record CreateMessageCommand: IRequest<Either<MessageException, Message>>
{
    public required string Content { get; init; }
    public required Guid ConversationId { get; init; }
}

public class CreateMessageCommandHandler(
    IConversationQueries conversationQueries,
    IMessageRepository messageRepository,
    ILlmService llmService,
    IHttpContextAccessor httpContextAccessor): IRequestHandler<CreateMessageCommand, Either<MessageException, Message>>
{
    public async Task<Either<MessageException, Message>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var conversationId = new ConversationId(request.ConversationId);
        var existingConversation = await conversationQueries.GetById(conversationId, cancellationToken);

        return await existingConversation.Match(
            async conversation =>
            {
                var sessionUserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(sessionUserId))
                {
                    return new MessageUserIdNotFoundException(Guid.Empty);
                }
                
                if (conversation.UserId != sessionUserId)
                {
                    return new MessageUserWrongException();
                }
                
                await CreateMessage(request.Content, conversationId, cancellationToken);

                try
                {
                    var response = await llmService.AskQuestionAsync(conversationId, request.Content, cancellationToken);
                    
                    return await CreateMessage(response, conversationId, cancellationToken, isResponse: true);
                }
                catch (Exception ex)
                {
                    return new MessagLlmException(ex);
                }
            },
            () => Task.FromResult<Either<MessageException, Message>>(new MessageConversationNotFoundException(request.ConversationId))
        );
    }
    
    private async Task<Either<MessageException,Message>> CreateMessage(string content, ConversationId conversationId,  CancellationToken cancellationToken, bool isResponse = false)
    {
        try
        {
            var entity = Message.New(content, conversationId, isResponse);
            
            return await messageRepository.Add(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            return new MessageUnknownException(Guid.Empty, ex);
        }
    }
}
