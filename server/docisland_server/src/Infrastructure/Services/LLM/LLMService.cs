using System.Text;
using System.Text.Json;
using Application.Common.Interfaces.Services.LLM;
using Domain.Conversations;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.LLM;

public class LlmService(HttpClient httpClient, IConfiguration configuration) : ILlmService
{
    private readonly string _llmEndpoint = configuration["LLMSettings:ServerUrl"];

    public async Task<string> AskQuestionAsync(ConversationId conversationId, string question, CancellationToken cancellationToken)
    {
        var askEndpoint = _llmEndpoint + "/ask";

        var prompt = LlmPromptTemplates.AskQuestionPrompt(conversationId, question);

        var payload = new
        {
            question = prompt
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(askEndpoint, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"LLM API request failed: {response.StatusCode} - {error}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return ollamaResponse?.Response ?? string.Empty;
    }

    public async Task CreateConversation(ConversationId conversationId, string context, CancellationToken cancellationToken)
    {
        var askEndpoint = _llmEndpoint + "/ask";

        var prompt = LlmPromptTemplates.CreateConversationPrompt(conversationId, context);

        var payload = new
        {
            question = prompt
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(askEndpoint, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"LLM API init failed: {response.StatusCode} - {error}");
        }
    }
}