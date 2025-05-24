using System.Text;
using System.Text.Json;
using Application.Common.Interfaces.Services.Files;
using Application.Common.Interfaces.Services.LLM;
using Domain.Files;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.LLM;

public class LlmService(HttpClient httpClient, IFileStorageService fileStorageService, IConfiguration configuration) : ILlmService
{
    private readonly string _llmEndpoint = configuration["LLMSettings:ServerUrl"];

    public async Task<string> AskQuestionAsync(Guid fileId, string question, CancellationToken cancellationToken)
    {
        var askEndpoint = _llmEndpoint + "/ask";
        
        const string conversationsFiles = "conversations-files";

        string? context = null;
        try
        {
            context = await fileStorageService.GetFileContentAsync(conversationsFiles, fileId, cancellationToken);
        }
        catch (Exception ex)
        {
            context = string.Empty;
        }
        var prompt = LlmPromptTemplates.AskQuestionPrompt(context!, question);

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
}