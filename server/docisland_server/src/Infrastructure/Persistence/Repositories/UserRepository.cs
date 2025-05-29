using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.Users;
using LanguageExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : IUserQueries
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<Option<User>> GetById(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return user == null ? Option<User>.None : Option<User>.Some(user);
    }

    public async Task<bool> IsAdmin(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        return await _userManager.IsInRoleAsync(user, "Admin");
    }

    public async Task<PaginatedResult<User>> GetPaginatedUsers(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = _userManager.Users.AsNoTracking();

        // Apply search if searchTerm is provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(u => 
                u.UserName!.ToLower().Contains(searchTerm) || 
                u.Email!.ToLower().Contains(searchTerm));
        }

        // Apply sorting
        query = sortBy?.ToLower() switch
        {
            "username" => sortDescending 
                ? query.OrderByDescending(u => u.UserName)
                : query.OrderBy(u => u.UserName),
            "email" => sortDescending 
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),
            _ => query.OrderBy(u => u.UserName)
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PaginatedResult<User>(
            items,
            totalCount,
            pageNumber,
            pageSize,
            totalPages);
    }
} 