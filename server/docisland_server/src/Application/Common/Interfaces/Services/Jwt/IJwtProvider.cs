using Domain.Roles;
using Domain.Users;

namespace Application.Common.Interfaces.Services.Jwt;

public interface IJwtProvider
{
    string Generate(User user, Role role);
}