using Domain.Conversations;

namespace Infrastructure.Services.LLM;

internal static class LlmPromptTemplates
{
    public static string AskQuestionPrompt(string context, string question) =>
        $"The context (also can be referred as file): {context}. Based on the context answer the following question:\n\n{question}";
}
