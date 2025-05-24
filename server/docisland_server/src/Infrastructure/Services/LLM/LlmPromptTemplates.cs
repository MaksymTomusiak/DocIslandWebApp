using Domain.Conversations;

namespace Infrastructure.Services.LLM;

internal static class LlmPromptTemplates
{
    public static string AskQuestionPrompt(string context, string question) =>
        $"The context: {context}. Based on the context answer the following question:\n\n{question}";
}
