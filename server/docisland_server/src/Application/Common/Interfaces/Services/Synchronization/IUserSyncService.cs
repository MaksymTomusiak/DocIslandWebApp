namespace Application.Common.Interfaces.Services.Synchronization;

public interface IUserSyncService
{
    Task SyncUserAsync(string userId, string? email, string? username);
    void ClearUserCache(string userId);
    Task SyncUserIsBannedStatusAsync(string userId, bool isBanned);
} 