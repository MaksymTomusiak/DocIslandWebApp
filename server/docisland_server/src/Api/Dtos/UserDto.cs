using Domain.Users;

namespace Api.Dtos;

public record UserDto(
    string Id,
    string UserName,
    string Email,
    bool IsAdmin,
    bool IsBanned)
{
    public static UserDto FromDomainModel(User user, bool isAdmin)
        => new(user.Id,
            user.UserName!, 
            user.Email!, 
            isAdmin,
            user.IsBanned);
}