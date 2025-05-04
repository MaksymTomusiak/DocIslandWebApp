using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services.Files;

public interface IFileTextExtractor
{
    Task<string> ExtractTextAsync(IFormFile file, CancellationToken cancellationToken);
    bool CanHandle(string contentType);
}