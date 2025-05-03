using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Files;

public static class FormFileExtensions
{
    private static readonly Dictionary<string, string> ContentTypeToExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        { "text/plain", ".txt" },
        { "application/pdf", ".pdf" },
        { "image/jpeg", ".jpg" },
        { "image/png", ".png" },
        { "application/json", ".json" },
        { "application/xml", ".xml" },
        { "text/csv", ".csv" },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx" }
    };

    public static string GetFileExtension(this IFormFile file, bool useFileNameFallback = true, string defaultExtension = ".bin")
    {
        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        // Try to infer extension from ContentType
        if (!string.IsNullOrEmpty(file.ContentType))
        {
            if (ContentTypeToExtension.TryGetValue(file.ContentType, out var extension))
            {
                return extension;
            }
        }

        // Fallback to FileName if enabled
        if (useFileNameFallback && !string.IsNullOrEmpty(file.FileName))
        {
            string extension = Path.GetExtension(file.FileName);
            if (!string.IsNullOrEmpty(extension))
            {
                return extension;
            }
        }

        // Return default extension
        return defaultExtension;
    }
}