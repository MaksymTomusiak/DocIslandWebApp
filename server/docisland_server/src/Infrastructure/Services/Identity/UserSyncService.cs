using Application.Common.Interfaces;
using Application.Common.Interfaces.Services.Synchronization;
using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services.Identity;

public class UserSyncService : IUserSyncService
{
    private readonly UserManager<User> _userManager;
    private readonly IMemoryCache _cache;
    private const string UserCacheKeyPrefix = "user_";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public UserSyncService(
        UserManager<User> userManager,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _cache = cache;
    }

    public void ClearUserCache(string userId)
    {
        var cacheKey = $"{UserCacheKeyPrefix}{userId}";
        _cache.Remove(cacheKey);
    }

    public async Task SyncUserIsBannedStatusAsync(string userId, bool isBanned)
    {
        var cacheKey = $"{UserCacheKeyPrefix}{userId}";
        
        if (_cache.TryGetValue<User>(cacheKey, out var cachedUser))
        {
            cachedUser.IsBanned = isBanned;
            _cache.Set(cacheKey, cachedUser, CacheDuration);
        }
    }

    public async Task SyncUserAsync(string userId, string? email, string? username)
    {
        var cacheKey = $"{UserCacheKeyPrefix}{userId}";
        var user = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await _userManager.FindByIdAsync(userId);
        });

        if (user == null)
        {
            // Check if this is the first user in the system
            var isFirstUser = !await _userManager.Users.AnyAsync();

            // Create new user
            user = new User
            {
                Id = userId,
                Email = email ?? $"{userId}@clerk.user",
                UserName = username ?? email ?? userId,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            if (isFirstUser)
            {
                // Make the first user an admin
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                // Add default role for other users
                await _userManager.AddToRoleAsync(user, "User");
            }

            // Cache the newly created user
            _cache.Set(cacheKey, user, CacheDuration);
        }
        else
        {
            // Check if user data has changed
            var needsUpdate = false;

            if (email != null && email != user.Email)
            {
                user.Email = email;
                needsUpdate = true;
            }

            if (username != null && username != user.UserName)
            {
                user.UserName = username;
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    throw new Exception($"Failed to update user: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
                }

                // Update cache
                _cache.Set(cacheKey, user, CacheDuration);
            }
        }
    }
} 