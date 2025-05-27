using Domain.Conversations;
using Microsoft.AspNetCore.Identity;
using File = Domain.Files.File;

namespace Domain.Users;

public class User : IdentityUser<string>
{
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiration { get; set; }
    public ICollection<File> Files { get; set; } = new List<File>();
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    public bool IsBanned { get; set; } = false;
}