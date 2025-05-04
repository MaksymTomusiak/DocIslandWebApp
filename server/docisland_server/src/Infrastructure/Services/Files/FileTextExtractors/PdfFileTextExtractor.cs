using System.Text;
using Application.Common.Interfaces.Services.Files;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Files.FileTextExtractors;

public class PdfFileTextExtractor : IFileTextExtractor
{
    public bool CanHandle(string contentType)
    {
        return contentType?.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) == true;
    }

    public async Task<string> ExtractTextAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        using var pdfReader = new PdfReader(memoryStream);
        using var pdfDocument = new PdfDocument(pdfReader);
        var text = new StringBuilder();
        for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
        {
            text.Append(PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i)));
        }
        return text.ToString();
    }
}