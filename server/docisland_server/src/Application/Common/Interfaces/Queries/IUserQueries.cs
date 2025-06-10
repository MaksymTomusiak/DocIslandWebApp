using Application.Common.Models;
using Domain.Users;

namespace Application.Common.Interfaces.Queries;

public interface IUserQueries
{
    Task<bool> IsAdmin(string userId, CancellationToken cancellationToken);
    Task<PaginatedResult<User>> GetPaginatedUsers(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        CancellationToken cancellationToken = default);
};