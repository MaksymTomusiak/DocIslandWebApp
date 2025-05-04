using System.Security.Claims;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services.Files;
using Application.Conversations.Exceptions;
using Domain.Conversations;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Application.Conversations.Commands;

public record DeleteConversationCommand : IRequest<Either<ConversationException, Conversation>>
{
    public required Guid ConversationId { get; init; }
}

public class DeleteConversationCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IConversationRepository conversationRepository,
    IConversationQueries conversationQueries,
    IFileQueries fileQueries,
    IFileStorageService fileStorageService,
    UserManager<User> userManager) : IRequestHandler<DeleteConversationCommand, Either<ConversationException, Conversation>>
{
    public async Task<Either<ConversationException, Conversation>> Handle(DeleteConversationCommand request,
        CancellationToken cancellationToken)
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

        var existingConversation = await conversationQueries.GetById(new ConversationId(request.ConversationId), cancellationToken);

        return await existingConversation.Match(
            async c =>
            {
                var isAdmin = await userManager.IsInRoleAsync(sessionUser, "Admin");
                if (!isAdmin && c.UserId != sessionUser.Id)
                {
                    return new ConversationCantBeDeletedException();
                }
                return await DeleteConversation(c, sessionUser.Id, cancellationToken);
            },
            () => Task.FromResult<Either<ConversationException, Conversation>>(new ConversationNotFoundException(request.ConversationId)));
    }

    private async Task<Either<ConversationException, Conversation>> DeleteConversation(Conversation conversation, Guid sessionUserId, CancellationToken cancellationToken)
    {
        try
        {
            const string conversationsFiles = "conversations-files";
            var conversationFile = await fileQueries.GetByConversation(conversation.Id, cancellationToken);
            var fileDeleteResult = await conversationFile.Match<Task<Either<ConversationException, bool>>>(
                async f =>
                {
                    try
                    {
                        await fileStorageService.DeleteFileAsync(conversationsFiles, f.Id.Value, cancellationToken);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return new ConversationFileDeletingException(ex);
                    }        
                },
                () => Task.FromResult<Either<ConversationException, bool>>(true));
            
            return await fileDeleteResult.Match<Task<Either<ConversationException, Conversation>>>(
                async _ => await conversationRepository.Delete(conversation, cancellationToken),
                ex => Task.FromResult<Either<ConversationException, Conversation>>(ex));
        }
        catch (Exception ex)
        {
            return new ConversationUnknownException(sessionUserId, ex);
        }
    }
}