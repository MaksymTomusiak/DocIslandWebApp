using System.Security.Claims;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services.Files;
using Application.Common.Interfaces.Services.LLM;
using Application.Conversations.Exceptions;
using Domain.Conversations;
using Domain.Files;
using Domain.Users;
using LanguageExt;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using File = Domain.Files.File;

namespace Application.Conversations.Commands;

public record CreateConversationCommand : IRequest<Either<ConversationException, Conversation>>
{
    public required IFormFile File { get; init; }
}

public class CreateConversationCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IConversationRepository conversationRepository,
    IFileRepository fileRepository,
    UserManager<User> userManager,
    IFileStorageService fileStorageService,
    IEnumerable<IFileTextExtractor> extractors,
    ILlmService llmService) : IRequestHandler<CreateConversationCommand, Either<ConversationException, Conversation>>
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

    private async Task<Either<ConversationException, Conversation>> CreateConversation(IFormFile file, string sessionUserId, CancellationToken cancellationToken)
    {
        try
        {
            var extractor = extractors.FirstOrDefault(e => e.CanHandle(file.ContentType));
            if (extractor == null)
            {
                return new ConversationUnsupportedFileTypeException(file.ContentType);
            }

            var extractedText = await extractor.ExtractTextAsync(file, cancellationToken);
            
            var fileEntity = File.New(file.FileName, (uint)file.Length, sessionUserId);
            const string conversationsFiles = "conversations-files";
            try
            {
                await fileStorageService.SaveFileAsync(file, conversationsFiles, fileEntity.Id.Value,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                return new ConversationFileSavingException(ex);
            }
            var conversation = Conversation.New(sessionUserId, fileEntity.Id);
            
            try 
            {
                await llmService.CreateConversation(conversation.Id, extractedText, cancellationToken);
            }
            catch (Exception ex)
            {
                return new ConversationLlmException(ex);
            }

            await fileRepository.Add(fileEntity, cancellationToken);
            
            return await conversationRepository.Add(conversation, cancellationToken);
        }
        catch (Exception ex)
        {
            return new ConversationUnknownException(String.Empty, ex);
        }
    }
}