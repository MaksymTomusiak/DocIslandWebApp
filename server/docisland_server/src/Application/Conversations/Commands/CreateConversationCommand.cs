using System.Security.Claims;
using Application.Common.Interfaces.Repositories;
using Application.Conversations.Exceptions;
using Domain.Conversations;
using Domain.Files;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Conversations.Commands;

public record CreateConversationCommand : IRequest<Either<ConversationException, Conversation>>
{
    public required IFormFile File { get; init; }
}

public class CreateConversationCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IConversationRepository conversationRepository,
    UserManager<User> userManager) : IRequestHandler<CreateConversationCommand, Either<ConversationException, Conversation>>
{
    public async Task<Either<ConversationException, Conversation>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var sessionUserId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(sessionUserId))
        {
            return new ConversationUserIdNotFoundException();
        }

        var sessionUser = await userManager.FindByIdAsync(sessionUserId);
        if (sessionUser == null)
        {
            return new ConversationUserNotFoundException();
        }

        return await CreateConversation(request.File, sessionUser.Id, cancellationToken);
    }

    private async Task<Either<ConversationException, Conversation>> CreateConversation(IFormFile file, Guid sessionUserId, CancellationToken cancellationToken)
    {
        try
        {
            //ToDo: Add file saving and getting fileId
            var fileId = FileId.Empty();
            
            var entity = Conversation.New(sessionUserId, fileId);
            
            return await conversationRepository.Add(entity, cancellationToken);
            
            //ToDo: Add initial message
        }
        catch (Exception ex)
        {
            return new ConversationUnknownException(Guid.Empty, ex);
        }
    }
}