using Domain.Conversations;

namespace Application.Common.Interfaces.Services.LLM;

public interface ILlmService
{
    Task<string> AskQuestionAsync(ConversationId conversationId, string question, CancellationToken cancellationToken);
    Task CreateConversation(ConversationId conversationId, string context, CancellationToken cancellationToken);
}