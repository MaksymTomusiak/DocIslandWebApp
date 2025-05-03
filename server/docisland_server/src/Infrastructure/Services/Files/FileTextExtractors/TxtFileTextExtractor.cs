using System.Text;
using Application.Common.Interfaces.Services.Files;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Files.FileTextExtractors;

public class TxtFileTextExtractor : IFileTextExtractor
{
    public bool CanHandle(string contentType)
    {
        return contentType?.StartsWith("text/plain", StringComparison.OrdinalIgnoreCase) == true;
    }

    public async Task<string> ExtractTextAsync(IFormFile file)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}