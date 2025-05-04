namespace Application.Common.Interfaces.Repositories;
using File = Domain.Files.File;
public interface IFileRepository
{
    Task<File> Add(File file, CancellationToken cancellationToken);
    Task<File> Update(File file, CancellationToken cancellationToken);
    Task<File> Delete(File file, CancellationToken cancellationToken);
}
