using Domain.Files;

namespace Application.Common.Interfaces.Services.LLM;

public interface ILlmService
{
    Task<string> AskQuestionAsync(Guid fileId, string question, CancellationToken cancellationToken);
}