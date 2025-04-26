using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services.Files;

public interface IFileStorageService
{
    Task<string?> SaveFileAsync(IFormFile file, string containerName, Guid id, CancellationToken cancellationToken);
    Task<string?> DeleteFileAsync(string containerName, Guid id, CancellationToken cancellationToken);
}