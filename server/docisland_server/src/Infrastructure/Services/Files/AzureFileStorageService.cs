using Application.Common.Interfaces.Services.Files;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Files;

public class AzureFileStorageService(BlobServiceClient blobServiceClient) : IFileStorageService
{
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
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Ensure container exists
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        // Determine file extension based on ContentType or FileName
        string fileExtension = file.GetFileExtension();

        // Construct blob name with the determined extension
        string blobName = $"{id}{fileExtension}";

        // Get blob client
        var blobClient = containerClient.GetBlobClient(blobName);

        // Upload file
        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);
        }

        // Set content type if provided
        if (!string.IsNullOrEmpty(file.ContentType))
        {
            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = file.ContentType
            };
            await blobClient.SetHttpHeadersAsync(blobHttpHeaders, cancellationToken: cancellationToken);
        }

        // Return the blob URI
        return blobClient.Uri.ToString();
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
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Since we don't know the extension, list blobs with the ID prefix
        string? blobName = null;
        await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: id.ToString(), cancellationToken: cancellationToken))
        {
            if (blobItem.Name.StartsWith(id.ToString()))
            {
                blobName = blobItem.Name;
                break;
            }
        }

        if (blobName == null)
        {
            return null; // Blob not found
        }

        // Get blob client and delete
        var blobClient = containerClient.GetBlobClient(blobName);
        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);

        // Return the deleted blob URI or null if not found
        return response.Value ? blobClient.Uri.ToString() : null;
    }
}