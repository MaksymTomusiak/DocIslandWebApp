using File = Domain.Files.File;

namespace Tests.Data;

public static class FilesData
{
    public static File NewFile(Guid userId) =>
        File.New(
            "Test file",
            502301,
            userId);
}