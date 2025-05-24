using Application.Common.Interfaces.Services.Files;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Infrastructure.Services.Files;

public class AzureFileStorageService(BlobServiceClient blobServiceClient, IEnumerable<IFileTextExtractor> extractors)
    : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
    private readonly IEnumerable<IFileTextExtractor> _extractors = extractors ?? throw new ArgumentNullException(nameof(extractors));

    public async Task<string?> SaveFileAsync(IFormFile file, string containerName, Guid id, CancellationToken cancellationToken)
    {
        // Validate inputs
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File cannot be null or empty.", nameof(file));
        }
        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentException("Container name cannot be empty.", nameof(containerName));
        }
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID cannot be empty.", nameof(id));
        }

        // Get container client
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        // Ensure container exists
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        // Determine file extension based on ContentType or FileName
        var fileExtension = file.GetFileExtension();

        // Construct blob name with the determined extension
        var blobName = $"{id}{fileExtension}";

        // Get blob client
        var blobClient = containerClient.GetBlobClient(blobName);

        // Upload file
        await using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);
        }

        // Set content type if provided
        if (!string.IsNullOrEmpty(file.ContentType))
        {
            var contentType = file.ContentType;
            // Add charset=utf-8 for text-based content types
            if (fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase) ||
                fileExtension.Equals(".json", StringComparison.OrdinalIgnoreCase) ||
                fileExtension.Equals(".xml", StringComparison.OrdinalIgnoreCase) ||
                fileExtension.Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                contentType = $"{contentType}; charset=utf-8";
            }
            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = contentType
            };
            await blobClient.SetHttpHeadersAsync(blobHttpHeaders, cancellationToken: cancellationToken);
        }

        // Save extracted text content if supported
        var extractor = file.ContentType != null
            ? _extractors.FirstOrDefault(e => e.CanHandle(file.ContentType))
            : null;
        if (extractor != null)
        {
            await SaveFileContentAsync(file, containerName, id, extractor, cancellationToken);
        }
        else
        {
            // Log for debugging
            Console.WriteLine($"No extractor found for ContentType: {file.ContentType ?? "null"}");
        }

        // Return the blob URI
        return blobClient.Uri.ToString();
    }

    private async Task SaveFileContentAsync(IFormFile file, string containerName, Guid id, IFileTextExtractor extractor, CancellationToken cancellationToken)
    {
        // Extract text
        var extractedText = await extractor.ExtractTextAsync(file, cancellationToken);

        // Get container client
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        // Construct content blob name (e.g., {id}_content.txt)
        var contentBlobName = $"{id}_content.txt";

        // Get blob client for content
        var contentBlobClient = containerClient.GetBlobClient(contentBlobName);

        // Upload extracted text as UTF-8
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(extractedText));
        await contentBlobClient.UploadAsync(stream, overwrite: true, cancellationToken);

        // Set content type with charset
        var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
        {
            ContentType = "text/plain; charset=utf-8"
        };
        await contentBlobClient.SetHttpHeadersAsync(blobHttpHeaders, cancellationToken: cancellationToken);
    }

    public async Task<string?> GetFileContentAsync(string containerName, Guid id, CancellationToken cancellationToken)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentException("Container name cannot be empty.", nameof(containerName));
        }
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID cannot be empty.", nameof(id));
        }

        // Get container client
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        // Construct content blob name
        var contentBlobName = $"{id}_content.txt";

        // Get blob client
        var blobClient = containerClient.GetBlobClient(contentBlobName);

        // Check if blob exists
        if (!await blobClient.ExistsAsync(cancellationToken))
        {
            return null; // Content not found
        }

        // Download and read text
        var blobDownloadInfo = await blobClient.DownloadAsync(cancellationToken);
        using var reader = new StreamReader(blobDownloadInfo.Value.Content, Encoding.UTF8);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    public async Task<string?> DeleteFileAsync(string containerName, Guid id, CancellationToken cancellationToken)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentException("Container name cannot be empty.", nameof(containerName));
        }
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID cannot be empty.", nameof(id));
        }

        // Get container client
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        // Delete original file (find blob with id prefix, excluding content blob)
        string? blobName = null;
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: id.ToString(), cancellationToken: cancellationToken))
        {
            if (blobItem.Name.StartsWith(id.ToString(), StringComparison.OrdinalIgnoreCase) && !blobItem.Name.EndsWith("_content.txt"))
            {
                blobName = blobItem.Name;
                break;
            }
        }

        string? deletedBlobUri = null;
        if (blobName != null)
        {
            var blobClient = containerClient.GetBlobClient(blobName);
            var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            if (response.Value)
            {
                deletedBlobUri = blobClient.Uri.ToString();
            }
        }

        // Delete content blob
        var contentBlobName = $"{id}_content.txt";
        var contentBlobClient = containerClient.GetBlobClient(contentBlobName);
        await contentBlobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

        // Return the deleted original blob URI or null if not found
        return deletedBlobUri;
    }
}