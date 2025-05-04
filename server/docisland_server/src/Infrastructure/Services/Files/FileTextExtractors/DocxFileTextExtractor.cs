using System.Text;
using Application.Common.Interfaces.Services.Files;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Files.FileTextExtractors;

public class DocxFileTextExtractor : IFileTextExtractor
{
    public bool CanHandle(string contentType)
    {
        return contentType?.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document", StringComparison.OrdinalIgnoreCase) == true;
    }

    public async Task<string> ExtractTextAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        using var wordDocument = WordprocessingDocument.Open(memoryStream, false);
        var text = new StringBuilder();
        foreach (var paragraph in wordDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>())
        {
            text.AppendLine(paragraph.InnerText);
        }
        return text.ToString();
    }
}