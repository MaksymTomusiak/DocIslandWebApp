using System.Text.Json.Serialization;

namespace Infrastructure.Services.LLM;

internal class OllamaResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; } = string.Empty;
}