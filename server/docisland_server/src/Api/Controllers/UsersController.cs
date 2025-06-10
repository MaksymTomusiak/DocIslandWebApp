using Api.Dtos;
using Api.Extensions;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Api.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController(UserManager<User> userManager, ISender sender, IUserQueries userQueries, IMemoryCache cache) : ControllerBase
{
    private const string UserCacheKeyPrefix = "user_";

    [HttpGet]
    [AdminAuthorize]
    public async Task<ActionResult<PaginatedResultDto<UserDto>>> GetAll(
        [FromQuery] PaginationParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var result = await userQueries.GetPaginatedUsers(
            parameters.PageNumber,
            parameters.PageSize,
            parameters.SearchTerm,
            parameters.SortBy,
            parameters.SortDescending,
            cancellationToken);

        var userDtos = new List<UserDto>();
        foreach (var user in result.Items)
        {
            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
            userDtos.Add(UserDto.FromDomainModel(user, isAdmin));
        }

        return Ok(new PaginatedResultDto<UserDto>(
            userDtos,
            result.TotalCount,
            result.PageNumber,
            result.PageSize,
            result.TotalPages));
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
        if (result.IsRight)
        {
            // Clear the user from cache to force a fresh fetch
            cache.Remove($"{UserCacheKeyPrefix}{userId}");
        }
        return result.Match(
            isBanned => Ok(new { isBanned }),
            error => error.ToObjectResult()
        );
    }
}