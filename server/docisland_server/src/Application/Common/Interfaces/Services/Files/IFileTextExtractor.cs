using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services.Files;

public interface IFileTextExtractor
{
    Task<string> ExtractTextAsync(IFormFile file);
    bool CanHandle(string contentType);
}