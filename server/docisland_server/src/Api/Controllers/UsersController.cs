using Api.Dtos;
using Api.Extensions;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Authorization;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController(UserManager<User> userManager, ISender sender, IUserQueries userQueries) : ControllerBase
{
    [HttpGet]
    [AdminAuthorize]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken cancellationToken = default)
    {
        var users = await userManager.Users.ToListAsync(cancellationToken);
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
            userDtos.Add(UserDto.FromDomainModel(user, isAdmin));
        }

        return Ok(userDtos);
    }

    [HttpGet("check-admin")]
    public async Task<IActionResult> CheckAdminStatus(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var isAdmin = await userQueries.IsAdmin(userId, cancellationToken);
        return Ok(new { isAdmin });
    }

    [HttpGet("check-ban")]
    public async Task<IActionResult> CheckBanStatus(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(new { isBanned = user.IsBanned });
    }

    [HttpPost("{userId}/toggle-admin")]
    [AdminAuthorize]
    public async Task<IActionResult> ToggleAdmin(string userId)
    {
        var result = await sender.Send(new ToggleUserAdminCommand { UserId = userId });
        return result.Match(
            isAdmin => Ok(new { isAdmin }),
            error => error.ToObjectResult()
        );
    }

    [HttpPost("{userId}/toggle-ban")]
    [AdminAuthorize]
    public async Task<IActionResult> ToggleBan(string userId)
    {
        var result = await sender.Send(new ToggleUserBanCommand { UserId = userId });
        return result.Match(
            isBanned => Ok(new { isBanned }),
            error => error.ToObjectResult()
        );
    }
}