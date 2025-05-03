namespace Application.Common.Interfaces.Services.LLM;

public interface ILlmService
{
    Task<string> AskQuestionAsync(string question);
}