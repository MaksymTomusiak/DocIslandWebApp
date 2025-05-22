using Api.Dtos;
using Api.Modules.Errors;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Route("users")]
[ApiController]
public class UsersController(
    UserManager<User> userManager,
    ISender sender) : ControllerBase
{
    private const string SuccessVerificationViewName = "VerifyEmailSuccess";
    private const string FacebookVerificationScheme = "Facebook";
    //ToDo: add cancellation token
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IEnumerable<UserDto>> GetAll(CancellationToken cancellationToken = default)
    {
        var users = await userManager.Users.ToListAsync(cancellationToken);
        
        return users.Select(u => UserDto.FromDomainModel(u));
    }
    
    [Authorize]
    [HttpDelete("delete/{userId}")]
    public async Task<ActionResult> Delete([FromRoute] string userId)
    {
        var command = new DeleteUserCommand { UserId = userId };
        var result = await sender.Send(command);
        return result.Match(Ok, e => e.ToObjectResult());
    }

    [Authorize]
    [HttpPut("update-password")]
    public async Task<ActionResult<UserDto>> UpdatePassword([FromBody] UserUpdatePasswordDto request)
    {
        var command = new UpdateUserPasswordCommand
        {
            OldPassword = request.OldPassword,
            NewPassword = request.NewPassword
        };
        var result = await sender.Send(command);
        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
    
    [Authorize]
    [HttpPut("update-username")]
    public async Task<ActionResult<UserDto>> UpdateUserName([FromBody] UserUpdateUserNameDto request)
    {
        var command = new UpdateUserNameCommand
        {
            UserName = request.UserName
        };
        var result = await sender.Send(command);
        return result.Match<ActionResult<UserDto>>(
            u => UserDto.FromDomainModel(u),
            e => e.ToObjectResult());
    }
}