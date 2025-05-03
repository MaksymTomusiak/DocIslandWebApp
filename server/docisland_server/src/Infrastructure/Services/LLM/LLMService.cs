using System.Text;
using System.Text.Json;
using Application.Common.Interfaces.Services.LLM;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.LLM;

public class LlmService(HttpClient httpClient, IConfiguration configuration) : ILlmService
{
    private readonly string _llmEndpoint = configuration["LLMSettings:ServerUrl"];

    public async Task<string> AskQuestionAsync(string question)
    {
        var askEndpoint = _llmEndpoint + "/ask";
        
        var payload = new
        {
            question
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(askEndpoint, content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"LLM API request failed: {response.StatusCode} - {error}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return ollamaResponse?.Response ?? string.Empty;
    }
}