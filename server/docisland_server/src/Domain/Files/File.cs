using Domain.Conversations;
using Domain.Users;

namespace Domain.Files;

public class File
{
    public FileId Id { get; set; }
    public string OriginalFileName { get; set; }
    public uint FileSizeBytes { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Conversation? Conversation { get; set; }

    private File(FileId id, string originalFileName, uint fileSizeBytes, Guid userId)
    {
        Id = id;
        OriginalFileName = originalFileName;
        FileSizeBytes = fileSizeBytes;
        UserId = userId;
    }

    public static File New(string originalFileName, uint fileSizeBytes, Guid userId) =>
        new(FileId.New(), originalFileName, fileSizeBytes, userId);
    
    public void UpdateDetails(string originalFileName, uint fileSizeBytes) =>
        (OriginalFileName, FileSizeBytes) = (originalFileName, fileSizeBytes);
}