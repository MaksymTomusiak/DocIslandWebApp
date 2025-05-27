namespace Application.Common.Interfaces.Queries;

public interface IUserQueries
{
    Task<bool> IsAdmin(string userId, CancellationToken cancellationToken);
};