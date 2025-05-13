using Domain.Conversations;

namespace Infrastructure.Services.LLM;

internal static class LlmPromptTemplates
{
    public static string CreateConversationPrompt(ConversationId conversationId, string context) =>
        $"This is the start of a new conversation with id {conversationId}.. Use the following context to understand the user:\n\n{context}";

    public static string AskQuestionPrompt(ConversationId conversationId, string question) =>
        $"This is question in the conversation with id {conversationId}. Answer the following question:\n\n{question}";
}
